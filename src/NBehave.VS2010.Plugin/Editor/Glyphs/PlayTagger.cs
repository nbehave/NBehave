using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.Narrator.Framework;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    public class PlayTagger : ITagger<PlayGlyphTag>
    {
        private readonly TokenParser tokenParser;
        private List<Feature> features = new List<Feature>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public PlayTagger(TokenParser tokenParser)
        {
            tokenParser.TokenParserEvent += UpdateEvents;
            this.tokenParser = tokenParser;
        }

        private void UpdateEvents(object sender, TokenParserEventArgs e)
        {
            if (e.Event.GherkinTokenType == GherkinTokenType.Eof)
                features = tokenParser.Features.ToList();
        }

        public IEnumerable<ITagSpan<PlayGlyphTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshot = (spans.Any()) ? spans.First().Snapshot : null;
            if (!features.Any() && textSnapshot != null)
                tokenParser.ForceParse(textSnapshot);

            var tagSPans = new List<ITagSpan<PlayGlyphTag>>();

            foreach (var line in textSnapshot.Lines)
            {
                var spanLine = line.LineNumber + 1;

                var scenario = features.SelectMany(_ => _.Scenarios).FirstOrDefault(_ => _.SourceLine == spanLine);
                if (scenario != null)
                {
                    var span = new SnapshotSpan(textSnapshot, line.Start, line.Length);
                    tagSPans.Add(new TagSpan<PlayGlyphTag>(span, new PlayGlyphTag(scenario)));
                }
            }
            return tagSPans;
        }
    }
}
