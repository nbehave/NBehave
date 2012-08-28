using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public class GherkinClassifier : ITagger<ClassificationTag>
    {
        private readonly Dictionary<GherkinTokenType, IClassificationType> gherkinTypes;
        private readonly ITagAggregator<GherkinTokenTag> aggregator;
        private readonly Queue<IMappingSpan> tagsChanged = new Queue<IMappingSpan>();

        public GherkinClassifier(ITagAggregator<GherkinTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
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
            NotifyTagChanged(e.Span.AnchorBuffer.CurrentSnapshot);
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
                NotifyTagChanged(spans[0].Snapshot);
        }

        private void NotifyTagChanged(ITextSnapshot snapshot)
        {
            IMappingSpan t = tagsChanged.Dequeue();
            NormalizedSnapshotSpanCollection sp = t.GetSpans(snapshot);
            if (TagsChanged != null)
                TagsChanged.Invoke(this, new SnapshotSpanEventArgs(sp.First()));
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
