using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Disposables;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.GherkinFileEditor.Glyphs;
using NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers;

namespace NBehave.VS2010.Plugin.GherkinFileEditor
{

    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        [Import]
        public ServiceRegistrar ServiceRegistrar { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            ServiceRegistrar.Initialise(buffer);
            
            return buffer.Properties.GetProperty<IClassifier>(typeof(GherkinFileClassifier));
        }
    }

    [Export(typeof(ServiceRegistrar))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ServiceRegistrar
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public void Initialise(ITextBuffer buffer)
        {
            if (!buffer.Properties.ContainsProperty(typeof(GherkinFileClassifier)))
            {
                var container = buffer.Properties.GetOrCreateSingletonProperty(() => new CompositionContainer(new AssemblyCatalog(typeof(NBehaveRunnerPackage).Assembly)));
                container.ComposeExportedValue(ClassificationRegistry);
                container.ComposeParts();

                container.GetExport<GherkinFileEditorParserFactory>().Value.CreateParser(buffer);

                GherkinFileClassifier fileClassifierForBuffer = buffer.Properties.GetOrCreateSingletonProperty(() => new GherkinFileClassifier(buffer));
                buffer.Properties.GetOrCreateSingletonProperty(() => new PlayTagger(buffer) as ITagger<PlayTag>);

                container.ComposeParts(fileClassifierForBuffer);
                fileClassifierForBuffer.BeginClassifications();
            }
        }
    }

    [Export(typeof(GherkinFileClassifier))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GherkinFileClassifier : IClassifier, IDisposable
    {
        private GherkinFileEditorParser _parser;
        private List<ClassificationSpan> _spans;
        private CompositeDisposable _listeners;

        public GherkinFileClassifier(ITextBuffer buffer)
        {
            _spans = new List<ClassificationSpan>();
            _listeners = new CompositeDisposable();

            _parser = buffer.Properties.GetProperty<GherkinFileEditorParser>(typeof (GherkinFileEditorParser));

            _listeners.Add(_parser.IsParsing.Where(isParsing => isParsing).Subscribe(b => _spans.Clear()));
            _listeners.Add(_parser.IsParsing.Where(isParsing => !isParsing).Subscribe(b => PublishClassificationEvents()));

            _listeners.Add(_parser
                            .ParserEvents
                            .Select(parserEvent => Classifiers
                                    .With(list => list.FirstOrDefault(classifier => classifier.CanClassify(parserEvent)))
                                    .Return(gherkinClassifier => gherkinClassifier.Classify(parserEvent), new List<ClassificationSpan>()))
                            .Subscribe((spans => _spans.AddRange(spans))));
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        
        [Import]
        public GherkinFileEditorParserFactory GherkinFileEditorParserFactory { get; set; }

        [ImportMany]
        public IEnumerable<IGherkinClassifier> Classifiers { get; set; }
        
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return _spans;
        }

        public void BeginClassifications()
        {
            _parser.FirstParse();
        }

        private void PublishClassificationEvents()
        {
            if (ClassificationChanged != null)
            {
                foreach (var classificationSpan in _spans)
                {
                    ClassificationChanged(this, new ClassificationChangedEventArgs(classificationSpan.Span));
                }
            }
        }

        public void Dispose()
        {
            _listeners.Dispose();
        }
    }
}
