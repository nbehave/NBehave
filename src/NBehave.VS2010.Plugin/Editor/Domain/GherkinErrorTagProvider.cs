using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.LanguageService;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("nbehave.gherkin")]
    [TagType(typeof(ErrorTag))]
    internal class GherkinErrorTagProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            ITagAggregator<GherkinTokenTag> tagAggregator = AggregatorFactory.CreateTagAggregator<GherkinTokenTag>(buffer);
            return new GherkinErrorTagger(tagAggregator) as ITagger<T>;
        }
    }
}