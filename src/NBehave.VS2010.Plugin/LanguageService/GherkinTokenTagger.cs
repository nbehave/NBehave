using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.LanguageService
{
    public class GherkinTokenTagger : ITagger<GherkinTokenTag>
    {
        private readonly GherkinStepTagger gherkinStepTagger;
        private readonly GherkinLanguageServiceListener gherkinLanguageServiceListener = new GherkinLanguageServiceListener();

        private List<GherkinParseEvent> events = new List<GherkinParseEvent>();
        private string lastParsedContent = "";
        private bool parseException;
        private string lastErrorTextEvent = "";

        public GherkinTokenTagger()
        {
            gherkinStepTagger = new GherkinStepTagger();
        }

        public IEnumerable<ITagSpan<GherkinTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tags = new List<ITagSpan<GherkinTokenTag>>();
            if (spans.Any())
            {
                var previousCallWasException = parseException;
                ParseEvents(spans.First());
                foreach (var span in spans)
                {
                    var thisParseFailedPrevoiusParseOk = (parseException && previousCallWasException == false);
                    var thisParseOkPreviousParseFailed = (!parseException && previousCallWasException);
                    tags.AddRange(spans.SelectMany(_ => gherkinStepTagger.CreateTags(events, _)));
                    var errorEvent = tags.FirstOrDefault(_ => _.Tag.Type == GherkinTokenType.SyntaxError);
                    if (!parseException)
                        lastErrorTextEvent = "";
                    if (thisParseFailedPrevoiusParseOk || thisParseOkPreviousParseFailed
                        || (errorEvent != null && errorEvent.Span.GetText() != lastErrorTextEvent && errorEvent.Span.GetText() == span.GetText()))
                    {
                        lastErrorTextEvent = (errorEvent != null) ? errorEvent.Span.GetText() : "";
                        RaiseTagsChanged(span);
                    }
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
            var content = span.Snapshot.GetText();
            if (content == lastParsedContent)
                return;
            try
            {
                Parse(content);
                parseException = false;
            }
            catch (Exception e)
            {
                parseException = true;
                var match = new Regex(@"^Line: (\d+).").Match(e.Message);
                if (!match.Success || !(e is ParseException))
                    return;
                var line = int.Parse(match.Groups[1].Value);
                var lineInFile = new LineInFile(line - 1);
                var token = new Token(span.GetText().TrimEnd(), lineInFile);
                var error = new Token(e.Message, lineInFile);
                events.Add(new GherkinParseEvent(GherkinTokenType.SyntaxError, token, error));
            }
            finally
            {
                lastParsedContent = content;
            }
        }

        private void Parse(string content)
        {
            events.Clear();
            gherkinLanguageServiceListener.Events.Clear();
            var parser = new Parser(gherkinLanguageServiceListener);
            try
            {
                parser.Scan(content);
            }
            finally
            {
                events = ToZeroBasedLines().ToList();
            }
        }

        private IEnumerable<GherkinParseEvent> ToZeroBasedLines()
        {
            return gherkinLanguageServiceListener.Events
                .Select(_ => new GherkinParseEvent(_.GherkinTokenType, _.Tokens.Select(t => new Token(t.Content, new LineInFile(t.LineInFile.Line - 1))).ToArray()));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}