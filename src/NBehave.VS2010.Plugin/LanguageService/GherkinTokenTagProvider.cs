using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.LanguageService
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

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new GherkinTokenTagger(buffer) as ITagger<T>;
        }
    }
}