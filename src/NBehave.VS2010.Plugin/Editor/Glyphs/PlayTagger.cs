using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Disposables;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.Narrator.Framework;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("gherkin")]
    [TagType(typeof(PlayGlyphTag))]
    public class PlayTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassifierAggregatorService AggregatorFactory = null;

        /// <summary>
        /// Creates an instance of our custom TodoTagger for a given buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">The buffer we are creating the tagger for.</param>
        /// <returns>An instance of our custom TodoTagger.</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetProperty<ITagger<T>>(typeof(ITagger<PlayGlyphTag>));
        }
    }

    /// <summary>
    /// This class implements ITagger for ToDoTag.  It is responsible for creating
    /// ToDoTag TagSpans, which our GlyphFactory will then create glyphs for.
    /// </summary>
    public class PlayTagger : ITagger<PlayGlyphTag>
    {
        private readonly CompositeDisposable listeners;
        private readonly List<ITagSpan<PlayGlyphTag>> tagSpans;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public PlayTagger(ITextBuffer buffer)
        {
            var currentSnapshot = buffer.CurrentSnapshot;
            var snapshotSpans = new Stack<SnapshotSpan>();
            listeners = new CompositeDisposable();
            tagSpans = new List<ITagSpan<PlayGlyphTag>>();

            var parser = buffer.Properties.GetProperty<GherkinFileEditorParser>(typeof(GherkinFileEditorParser));

            listeners.Add(parser.IsParsing.Where(_ => _)
                .Subscribe(b1 => tagSpans.Clear()));

            listeners.Add(parser.IsParsing.Where(_ => !_)
                .Subscribe(_ => ConvertSnapshotSpanToPlayGlyphTag(snapshotSpans)));

            listeners.Add(parser.ParserEvents
                .Where(parserEvent => true)
                .Subscribe(parserEvent => ScenariosToSnapshotSpan(currentSnapshot, parserEvent).ToList().ForEach(snapshotSpans.Push)));
        }

        private void ConvertSnapshotSpanToPlayGlyphTag(Stack<SnapshotSpan> snapshotSpans)
        {
            while (!snapshotSpans.IsEmpty())
            {
                var snapshotSpan = snapshotSpans.Pop();
                var playGlyphTag = new PlayGlyphTag(snapshotSpan.GetText());
                tagSpans.Add(new TagSpan<PlayGlyphTag>(snapshotSpan, playGlyphTag));
            }
            tagSpans.Reverse();
        }

        private static IEnumerable<SnapshotSpan> ScenariosToSnapshotSpan(ITextSnapshot currentSnapshot, Feature parserEvent)
        {
            var scenarios = parserEvent.Scenarios;
            var numberOfScenarios = scenarios.Count;
            for (int i = 0; i < numberOfScenarios; i++)
            {
                var snapshotSpan = ScenarioToSnapshotSpan(scenarios, i, currentSnapshot);
                yield return snapshotSpan;
            }
        }

        private static SnapshotSpan ScenarioToSnapshotSpan(List<Scenario> scenarios, int currentIndex, ITextSnapshot currentSnapshot)
        {
            var numberOfScenarios = scenarios.Count;
            ITextSnapshotLine start = currentSnapshot.GetLineFromLineNumber(scenarios[currentIndex].SourceLine - 1);
            var end = (currentIndex + 1 < numberOfScenarios)
                          ? currentSnapshot.GetLineFromLineNumber(scenarios[currentIndex + 1].SourceLine - 1).Start
                          : currentSnapshot.GetLineFromLineNumber(currentSnapshot.LineCount - 1).End;
            var snapshotSpan = new SnapshotSpan(start.Start, end - start.Start);
            return snapshotSpan;
        }

        /// <summary>
        /// This method creates ToDoTag TagSpans over a set of SnapshotSpans.
        /// </summary>
        /// <param name="spans">A set of spans we want to get tags for.</param>
        /// <returns>The list of ToDoTag TagSpans.</returns>
        IEnumerable<ITagSpan<PlayGlyphTag>> ITagger<PlayGlyphTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return tagSpans;
        }
    }
}
