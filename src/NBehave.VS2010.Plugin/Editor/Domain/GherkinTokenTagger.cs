using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public class GherkinTokenTagger : ITagger<GherkinTokenTag>
    {
        private readonly GherkinStepTagger gherkinStepTagger;
        private readonly TokenParser tokenParser;

        public GherkinTokenTagger(TokenParser tokenParser)
        {
            gherkinStepTagger = new GherkinStepTagger();
            this.tokenParser = tokenParser;
            tokenParser.TokenParserEvent += UpdateEvents;
        }

        private void UpdateEvents(object sender, TokenParserEventArgs e)
        {
            TagsChanged.Invoke(this, new SnapshotSpanEventArgs(e.SnapshotSpan));
        }

        public IEnumerable<ITagSpan<GherkinTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = new List<ITagSpan<GherkinTokenTag>>();
            if (spans.Any())
            {
                var events = tokenParser.Events;
                tags.AddRange(spans.SelectMany(_ => gherkinStepTagger.CreateTags(events, _)));
            }
            return tags;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}