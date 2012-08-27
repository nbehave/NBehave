using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("nbehave.gherkin")]
    [TagType(typeof(GherkinTokenTag))]
    internal class GherkinTokenTagProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory = null;

        [Import]
        internal TokenParserFactory TokenParserFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var tp = TokenParserFactory.Build(buffer);
            return new GherkinTokenTagger(tp) as ITagger<T>;
        }
    }
}