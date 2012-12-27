using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Editor.Domain;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    public class PlayTagger : ITagger<PlayGlyphTag>
    {
        private readonly TokenParser tokenParser;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        List<Feature> features = new List<Feature>();

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
            if (!features.Any() && textSnapshot != null && !tokenParser.LastParseFailed())
                tokenParser.ForceParse(textSnapshot);

            var tagSPans = new List<ITagSpan<PlayGlyphTag>>();
            foreach (var line in textSnapshot.Lines)
            {
                var spanLine = line.LineNumber + 1;

                AddFeatureTagSpan(spanLine, textSnapshot, line, tagSPans);
                AddScenarioTagSpan(spanLine, textSnapshot, line, tagSPans);
            }
            return tagSPans;
        }

        private void AddFeatureTagSpan(int spanLine, ITextSnapshot textSnapshot, ITextSnapshotLine line, List<ITagSpan<PlayGlyphTag>> tagSPans)
        {
            var feature = features.FirstOrDefault(_=> _.SourceLine == spanLine);
            if (feature != null)
            {
                var span = new SnapshotSpan(textSnapshot, line.Start, line.Length);
                tagSPans.Add(new TagSpan<PlayGlyphTag>(span, new PlayGlyphTag(new FeatureGherkinText(feature))));
            }
        }

        private void AddScenarioTagSpan(int spanLine, ITextSnapshot textSnapshot, ITextSnapshotLine line, List<ITagSpan<PlayGlyphTag>> tagSPans)
        {
            var scenario = features.SelectMany(_ => _.Scenarios).FirstOrDefault(_ => _.SourceLine == spanLine);
            if (scenario != null)
            {
                var span = new SnapshotSpan(textSnapshot, line.Start, line.Length);
                tagSPans.Add(new TagSpan<PlayGlyphTag>(span, new PlayGlyphTag( new ScenarioGherkinText(scenario))));
            }
        }
    }
}
