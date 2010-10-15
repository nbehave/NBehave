using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.GherkinFileEditor
{

    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        [Import]
        private GherkinFileClassifier GherkinFileClassifier { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            GherkinFileClassifier.InitialiseWithBuffer(buffer);
            return buffer.Properties.GetOrCreateSingletonProperty(() => GherkinFileClassifier);
        }

    }

    [Export(typeof(GherkinFileClassifier))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GherkinFileClassifier : IClassifier, IDisposable
    {
        private GherkinFileEditorParser _parser;
        private List<ClassificationSpan> _spans;
        private IDisposable _featureListener;
        private bool _finishedParsing;
        private IDisposable _eofListener;
        private IDisposable _startListener;
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public GherkinFileClassifier()
        {
            _spans = new List<ClassificationSpan>();
        }

        [Import]
        public GherkinFileEditorParserFactory GherkinFileEditorParserFactory { get; set; }

        [Import]
        public GherkinFileEditorClassifications ClassificationRegistry { get; set; }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return _spans;
        }

        public void InitialiseWithBuffer(ITextBuffer buffer)
        {
            if (_parser != null)
                return;

            _spans.Clear();

            _parser = GherkinFileEditorParserFactory.CreateParser(buffer); ;

            _startListener = _parser.ParserEvents.Where(event2 => event2.EventType != ParserEventType.Eof)
                .Subscribe(parserEvent2 =>
                {
                    if (_finishedParsing)
                    {
                        _finishedParsing = false;
                        _spans.Clear();
                    }
                });
            _eofListener = _parser.ParserEvents.Where(event1 => event1.EventType == ParserEventType.Eof)
                .Subscribe(parserEvent1 =>
                               {

                                   foreach (var classificationSpan in _spans)
                                   {
                                       if (ClassificationChanged != null)
                                       {
                                           ClassificationChanged(this, new ClassificationChangedEventArgs(classificationSpan.Span));
                                       }
                                   }

                                   _finishedParsing = true;
                               }
                );

            IObservable<ClassificationSpan[]> observable = _parser.ParserEvents
                .Where(@event => @event.EventType == ParserEventType.Feature)
                .Select(parserEvent => new[]
                                           {
                                               new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, parserEvent.KeywordSpan), ClassificationRegistry.Keyword),
                                               new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, parserEvent.TitleSpan), ClassificationRegistry.FeatureTitle),
                                               new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, parserEvent.DescriptionSpan), ClassificationRegistry.Description),
                                           });
            
            
            
            _featureListener = observable.Subscribe((spans => _spans.AddRange(spans)));



//
//
//            , () =>
//                                                              {
//         if (ClassificationChanged != null)
//                                {
//                                                                  foreach (var classificationSpan in _spans)
//                                                                  {
//                                                                  }
//                                                                  ClassificationChanged(this, null);
//
//                                }
//
//                                                              }
            _parser.FirstParse();
        }

        public void Dispose()
        {
            _featureListener.Dispose();
        }
    }
}
