using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.LanguageService;

namespace NBehave.VS2010.Plugin.Editor.Domain
{

    [Export(typeof(ITaggerProvider))]
    [ContentType("nbehave.gherkin")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class GherkinClassifierProvider : ITaggerProvider
    {
        [Export]
        [Name("nbehave.gherkin")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition GherkinContentTypeDefinition = null;

        [Export]
        [FileExtension(".feature")]
        [ContentType("nbehave.gherkin")]
        internal static FileExtensionToContentTypeDefinition GherkinFileExtensionDefinition = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            ITagAggregator<GherkinTokenTag> tagAggregator = AggregatorFactory.CreateTagAggregator<GherkinTokenTag>(buffer);
            return new GherkinClassifier(buffer, tagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    public class GherkinClassifier : ITagger<ClassificationTag>
    {
        private readonly ITextBuffer buffer;
        private readonly Dictionary<GherkinTokenType, IClassificationType> gherkinTypes;
        private readonly ITagAggregator<GherkinTokenTag> aggregator;
        private readonly Queue<IMappingSpan> tagsChanged = new Queue<IMappingSpan>();

        public GherkinClassifier(ITextBuffer buffer,
            ITagAggregator<GherkinTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
            this.buffer = buffer;
            this.aggregator = aggregator;
            aggregator.TagsChanged += TagsChangedOnAggregator;
            gherkinTypes = new Dictionary<GherkinTokenType, IClassificationType>();
            gherkinTypes[GherkinTokenType.SyntaxError] = typeService.GetClassificationType("gherkin.syntaxError");
            gherkinTypes[GherkinTokenType.Feature] = typeService.GetClassificationType("gherkin.feature");
            gherkinTypes[GherkinTokenType.FeatureTitle] = typeService.GetClassificationType("gherkin.featureTitle");
            gherkinTypes[GherkinTokenType.FeatureDescription] = typeService.GetClassificationType("gherkin.featureDescription");
            gherkinTypes[GherkinTokenType.Scenario] = typeService.GetClassificationType("gherkin.scenario");
            gherkinTypes[GherkinTokenType.ScenarioTitle] = typeService.GetClassificationType("gherkin.scenarioTitle");
            gherkinTypes[GherkinTokenType.Background] = typeService.GetClassificationType("gherkin.background");
            gherkinTypes[GherkinTokenType.BackgroundTitle] = typeService.GetClassificationType("gherkin.backgroundTitle");
            gherkinTypes[GherkinTokenType.Comment] = typeService.GetClassificationType("gherkin.comment");
            gherkinTypes[GherkinTokenType.Tag] = typeService.GetClassificationType("gherkin.tag");
            gherkinTypes[GherkinTokenType.DocString] = typeService.GetClassificationType("gherkin.docstring");
            gherkinTypes[GherkinTokenType.Examples] = typeService.GetClassificationType("gherkin.examples");
            gherkinTypes[GherkinTokenType.Step] = typeService.GetClassificationType("gherkin.step");
            gherkinTypes[GherkinTokenType.StepText] = typeService.GetClassificationType("gherkin.stepText");
            gherkinTypes[GherkinTokenType.Table] = typeService.GetClassificationType("gherkin.table");
            gherkinTypes[GherkinTokenType.TableHeader] = typeService.GetClassificationType("gherkin.tableheader");
            gherkinTypes[GherkinTokenType.TableCell] = typeService.GetClassificationType("gherkin.tablecell");
        }

        private void TagsChangedOnAggregator(object sender, TagsChangedEventArgs e)
        {
            tagsChanged.Enqueue(e.Span);
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = aggregator.GetTags(spans).ToList();
            foreach (var tagSpan in tags)
                yield return CreateTagSpanForTag(spans[0].Snapshot, tagSpan);
            NotifyTagsChanged(spans);
        }

        private void NotifyTagsChanged(NormalizedSnapshotSpanCollection spans)
        {
            while (tagsChanged.Any())
            {
                IMappingSpan t = tagsChanged.Dequeue();
                NormalizedSnapshotSpanCollection sp = t.GetSpans(spans[0].Snapshot);
                TagsChanged.Invoke(this, new SnapshotSpanEventArgs(sp.First()));
            }
        }

        private ITagSpan<ClassificationTag> CreateTagSpanForTag(ITextSnapshot snapShot, IMappingTagSpan<GherkinTokenTag> tagSpan)
        {
            NormalizedSnapshotSpanCollection tagSpans = tagSpan.Span.GetSpans(snapShot);
            GherkinTokenType classificationType = tagSpan.Tag.Type;
            return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(gherkinTypes[classificationType]));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
