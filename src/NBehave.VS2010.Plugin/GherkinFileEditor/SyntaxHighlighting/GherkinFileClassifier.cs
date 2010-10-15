using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers;

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
        private IDisposable _classificationsListener;
        private bool _finishedParsing;
        private IDisposable _eofListener;
        private IDisposable _startListener;
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        
        [Import]
        public GherkinFileEditorParserFactory GherkinFileEditorParserFactory { get; set; }

        [ImportMany]
        public IList<IGherkinClassifier> Classifiers { get; set; }
        
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return _spans;
        }

        public void InitialiseWithBuffer(ITextBuffer buffer)
        {
            if (_parser != null)
                return;

            _spans = new List<ClassificationSpan>();

            _parser = GherkinFileEditorParserFactory.CreateParser(buffer); ;

            _startListener = _parser.ParserEvents
                .Where(event2 => event2.EventType != ParserEventType.Eof)
                .Subscribe(parserEvent2 =>
                {
                    if (_finishedParsing)
                    {
                        _finishedParsing = false;
                        _spans.Clear();
                    }
                });

            _eofListener = _parser.ParserEvents
                .Where(event1 => event1.EventType == ParserEventType.Eof)
                .Subscribe(parserEvent1 =>
                {
                    if (ClassificationChanged != null)
                    {
                        foreach (var classificationSpan in _spans)
                        {
                            ClassificationChanged(this, new ClassificationChangedEventArgs(classificationSpan.Span));
                        }
                    }
                    _finishedParsing = true;
                });


            _classificationsListener = _parser
                                        .ParserEvents
                                        .Select(parserEvent => Classifiers
                                                .With(list => list.FirstOrDefault(classifier => classifier.CanClassify(parserEvent)))
                                                .Return(gherkinClassifier => gherkinClassifier.Classify(parserEvent), new List<ClassificationSpan>()))
                                        .Subscribe((spans => _spans.AddRange(spans)));

            _parser.FirstParse();
        }

        public void Dispose()
        {
            _eofListener.Dispose();
            _startListener.Dispose();
            _classificationsListener.Dispose();
        }
    }
}
