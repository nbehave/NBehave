using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.LanguageService
{
    public class GherkinTokenTagger : ITagger<GherkinTokenTag>
    {
        private List<GherkinParseEvent> events = new List<GherkinParseEvent>();
        private readonly GherkinStepTagger gherkinStepTagger;
        private readonly GherkinLanguageServiceListener gherkinLanguageServiceListener = new GherkinLanguageServiceListener();
        private bool parseException;

        public GherkinTokenTagger(ITextBuffer buffer)
        {
            gherkinStepTagger = new GherkinStepTagger(buffer);
        }

        public IEnumerable<ITagSpan<GherkinTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = new List<ITagSpan<GherkinTokenTag>>();
            if (spans.Any())
            {
                var previousCallWasException = parseException;
                var span = spans.First();
                ParseEvents(span);
                tags.AddRange(spans.SelectMany(_ => gherkinStepTagger.CreateTags(events, _)));
                if (!parseException && previousCallWasException)
                {
                    RaiseTagsChanged(span);
                }
            }
            return tags;
        }

        private void RaiseTagsChanged(SnapshotSpan span)
        {
            var lines = span.Snapshot.Lines;
            lines
                .Select(l => new SnapshotSpan(span.Snapshot, l.Start, l.Length))
                .ToList()
                .ForEach(s => TagsChanged.Invoke(this, new SnapshotSpanEventArgs(s)));
        }

        private void ParseEvents(SnapshotSpan span)
        {
            try
            {
                Parse(span.Snapshot.GetText());
                parseException = false;
            }
            catch (Exception e)
            {
                bool previousParseFailed = parseException;
                parseException = true;
                var match = new Regex(@"\d+$").Match(e.Message);
                if (!match.Success || !(e is ParseException) || previousParseFailed)
                    return;
                var line = int.Parse(match.Value);
                var lineInFile = new LineInFile(line - 1);
                var token = new Token(span.GetText().Split(new[] { '\n' })[lineInFile.Line].TrimEnd(), lineInFile);
                var error = new Token(e.Message, lineInFile);
                events.Add(new GherkinParseEvent(GherkinTokenType.SyntaxError, token, error));
            }
        }

        private void Parse(string content)
        {
            gherkinLanguageServiceListener.Events.Clear();
            var parser = new Parser(gherkinLanguageServiceListener);
            parser.Scan(content);
            events = ToZeroBasedLines().ToList();
        }

        private IEnumerable<GherkinParseEvent> ToZeroBasedLines()
        {
            return gherkinLanguageServiceListener.Events
                .Select(_ => new GherkinParseEvent(_.GherkinTokenType, _.Tokens.Select(t => new Token(t.Content, new LineInFile(t.LineInFile.Line - 1))).ToArray()));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    public class GherkinTokenTag : ITag
    {
        public GherkinTokenType Type { get; private set; }

        public GherkinTokenTag(GherkinTokenType type)
        {
            this.Type = type;
        }
    }
}
