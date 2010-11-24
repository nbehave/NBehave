//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Classification;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio.Text.Tagging;
//using Microsoft.VisualStudio.Utilities;
//
//namespace NBehave.VS2010.Plugin.GherkinFileEditor
//{
//
//    internal class ScenarioTag : IGlyphTag { }
//
//    [Export(typeof(ITaggerProvider))]
//    [TagType(typeof(ScenarioTag))]
//    [ContentType("gherkin")]
//    internal sealed class ScenarioTaggerProvider : ITaggerProvider
//    {
//        [Import]
//        internal IClassificationTypeRegistryService ClassificationRegistry;
//
//        protected GherkinFileEditorParser GetParser(ITextBuffer buffer)
//        {
//            return GherkinFileEditorParser.GetParser(buffer, ClassificationRegistry);
//        }
//
//        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
//        {
//            GherkinFileEditorParser parser = GetParser(buffer);
//
//            return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() => new ScenarioTagger(parser));
//        }
//    }
//
//    internal class ScenarioTagger : ITagger<ScenarioTag>
//    {
//        private readonly GherkinFileEditorParser parser;
//
//        public ScenarioTagger(GherkinFileEditorParser parser)
//        {
//            this.parser = parser;
//
//            parser.TagsChanged += (sender, args) =>
//            {
//                if (TagsChanged != null)
//                    TagsChanged(this, args);
//            };
//        }
//
//        public IEnumerable<ITagSpan<ScenarioTag>> GetTags(NormalizedSnapshotSpanCollection spans)
//        {
//            throw new NotImplementedException();
//        }
//
//        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
//    }
//}
