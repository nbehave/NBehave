//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Classification;
//using Microsoft.VisualStudio.Text.Tagging;
//using Microsoft.VisualStudio.Utilities;
//
//namespace NBehave.VS2010.Plugin.GherkinFileEditor
//{
//    [Export(typeof(ITaggerProvider))]
//    [TagType(typeof(IOutliningRegionTag))]
//    [ContentType("gherkin")]
//    internal sealed class OutliningTaggerProvider : ITaggerProvider
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
//            return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() =>
//                new GherkinFileOutliningTagger(parser));
//        }
//    }
//
//    internal class GherkinFileOutliningTagger : ITagger<IOutliningRegionTag>
//    {
//        private readonly GherkinFileEditorParser parser;
//
//        public GherkinFileOutliningTagger(GherkinFileEditorParser parser)
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
//        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
//        {
//            throw new NotImplementedException();
//        }
//
//        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
//    }
//}
