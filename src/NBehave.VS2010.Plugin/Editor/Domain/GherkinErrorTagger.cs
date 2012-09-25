using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public class GherkinErrorTagger : ITagger<IErrorTag>
    {
        private readonly ITagAggregator<GherkinTokenTag> aggregator;

        public GherkinErrorTagger(ITagAggregator<GherkinTokenTag> aggregator, TokenParser tokenParser)
        {
            this.aggregator = aggregator;
            tokenParser.TokenParserEvent += UpdateEvents;
        }

        private void UpdateEvents(object sender, TokenParserEventArgs e)
        {
            if (e.Event.GherkinTokenType == GherkinTokenType.SyntaxError)
                TagsChanged.Invoke(this, new SnapshotSpanEventArgs(e.SnapshotSpan));
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = aggregator.GetTags(spans).ToList();
            var errorTags = tags
                .Where(_ => _.Tag.Type == GherkinTokenType.SyntaxError)
                .ToList();
            foreach (var tagSpan in errorTags)
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                //TODO: Add tooltip & get error from parser
                string errorType = "Failed to parse text!!";
                var t = new ErrorTag(errorType);
                yield return new TagSpan<ErrorTag>(tagSpans[0], t);
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}