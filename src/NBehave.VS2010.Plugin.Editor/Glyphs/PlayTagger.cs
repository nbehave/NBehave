using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Disposables;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("gherkin")]
    [TagType(typeof(PlayGlyphTag))]
    public class PlayTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassifierAggregatorService AggregatorFactory;

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
        private readonly CompositeDisposable _listeners;
        private readonly List<ITagSpan<PlayGlyphTag>> _tagSpans;
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        private readonly Stack<SnapshotSpan> _snapshotSpans = new Stack<SnapshotSpan>();

        public PlayTagger(ITextBuffer buffer, ScenarioRunner scenarioRunner)
        {
            _listeners = new CompositeDisposable();
            _tagSpans = new List<ITagSpan<PlayGlyphTag>>();

            var parser = buffer.Properties.GetProperty<GherkinFileEditorParser>(typeof (GherkinFileEditorParser));

            _listeners.Add(parser.IsParsing.Where(b => b).Subscribe(b1 => _tagSpans.Clear()));

            _listeners.Add(parser.IsParsing.Where(b2 => !b2).Subscribe(b3 =>
            {
                while (!_snapshotSpans.IsEmpty())
                {
                    var snapshotSpan = _snapshotSpans.Pop();
                    var playGlyphTag = new PlayGlyphTag(snapshotSpan.GetText());
                    _tagSpans.Add(new TagSpan<PlayGlyphTag>(snapshotSpan, playGlyphTag));
                }
                _tagSpans.Reverse();
            }));

            _listeners.Add(parser
                .ParserEvents
                .Where(parserEvent => parserEvent.EventType == ParserEventType.Scenario)
                .Subscribe(parserEvent =>
                {
                    ITextSnapshotLine lineFromLineNumber = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);
                    _snapshotSpans.Push(new SnapshotSpan(lineFromLineNumber.Start, lineFromLineNumber.Length));
                }));

            _listeners.Add(parser
                .ParserEvents
                .Where(parserEvent => parserEvent.EventType == ParserEventType.Step || parserEvent.EventType == ParserEventType.Examples)
                .Subscribe(parserEvent =>
                {
                    ITextSnapshotLine lineFromLineNumber = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);
                    var snapshotSpan = _snapshotSpans.Pop();
                    _snapshotSpans.Push(new SnapshotSpan(snapshotSpan.Start, (lineFromLineNumber.Start.Position - snapshotSpan.Start.Position + lineFromLineNumber.Length)));
                }));

            _listeners.Add(parser
                .ParserEvents
                .Where(parserEvent => parserEvent.EventType == ParserEventType.Table)
                .Subscribe(parserEvent =>
                {
                    ITextSnapshotLine lastRowLine = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line + parserEvent.RowCount - 1);
                    var snapshotSpan = _snapshotSpans.Pop();
                    int lastLineReturn = lastRowLine.GetText().EndsWith(Environment.NewLine) ? Environment.NewLine.Length : 0;
                    _snapshotSpans.Push(new SnapshotSpan(snapshotSpan.Start, (lastRowLine.Start.Position - snapshotSpan.Start.Position) + lastRowLine.Length - lastLineReturn));
                }));

        }

        /// <summary>
        /// This method creates ToDoTag TagSpans over a set of SnapshotSpans.
        /// </summary>
        /// <param name="spans">A set of spans we want to get tags for.</param>
        /// <returns>The list of ToDoTag TagSpans.</returns>
        IEnumerable<ITagSpan<PlayGlyphTag>> ITagger<PlayGlyphTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tagSpans;
        }
    }
}
