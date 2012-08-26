using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.LanguageService
{
    public class GherkinStepTagger
    {
        private static readonly char[] WhiteSpaces = new[] { '\t', ' ', '\r', '\n' };

        private readonly Dictionary<GherkinTokenType, Func<SnapshotSpan, GherkinParseEvent, IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>>>> tagHandler;
        public GherkinStepTagger()
        {
            tagHandler = new Dictionary<GherkinTokenType, Func<SnapshotSpan, GherkinParseEvent, IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>>>>
                {
                    {GherkinTokenType.SyntaxError, SyntaxError},
                    {GherkinTokenType.Feature, HandleFeature},
                    {GherkinTokenType.Scenario, HandleScenario},
                    {GherkinTokenType.Background, HandleBackground},
                    {GherkinTokenType.Comment, HandleComment},
                    {GherkinTokenType.Tag, HandleTag},
                    {GherkinTokenType.DocString, NotImplemented},
                    {GherkinTokenType.Examples, NotImplemented},
                    {GherkinTokenType.Step, HandleStep},
                    {GherkinTokenType.Table, NotImplemented},
                    {GherkinTokenType.Eof, NotImplemented},
                };
        }

        public IEnumerable<ITagSpan<GherkinTokenTag>> CreateTags(List<GherkinParseEvent> events, SnapshotSpan span)
        {
            var value = span.GetText();
            if (string.IsNullOrWhiteSpace(value))
                return new ITagSpan<GherkinTokenTag>[0];

            var tags = new List<ITagSpan<GherkinTokenTag>>();
            var line = span.Start.GetContainingLine();
            int lineNumber = line.LineNumber;

            var evt = FindMatchingEvent(events, lineNumber);
            if (evt != null)
            {
                Func<SnapshotSpan, GherkinParseEvent, IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>>> tagger;
                if (tagHandler.TryGetValue(evt.GherkinTokenType, out tagger))
                {
                    var items = tagger(span, evt).ToList();
                    var intersectingTags = items.Where(_ => _.Item1.IntersectsWith(span)).Select(_ => _.Item2);
                    tags.AddRange(intersectingTags);
                }
            }
            return tags;
        }

        private GherkinParseEvent FindMatchingEvent(List<GherkinParseEvent> events, int lineNumber)
        {
            var evt = events.FirstOrDefault(_ => _.Tokens.Any(λ => λ.LineInFile.Line == lineNumber))
                      ?? events.Where(_ => _.Tokens.Any(λ => λ.LineInFile.Line < lineNumber))
                             .OrderByDescending(_ => _.Tokens.First().LineInFile.Line)
                             .FirstOrDefault();
            if (evt != null && evt.GherkinTokenType == GherkinTokenType.SyntaxError && lineNumber > evt.Tokens.Max(_ => _.LineInFile.Line))
                return null;
            return evt;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> NotImplemented(SnapshotSpan span, GherkinParseEvent evt)
        {
            yield break;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleFeature(SnapshotSpan span, GherkinParseEvent evt)
        {
            var result = HandleIt(span, evt, GherkinTokenType.FeatureTitle).ToList();
            if (evt.Tokens.Count == 3)
            {
                Token token = evt.Tokens.Last();
                string text = span.GetText().TrimEnd(WhiteSpaces);
                if (!string.IsNullOrWhiteSpace(text) && token.Content.Contains(text))
                {
                    ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                    var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position, text.Length));
                    var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(GherkinTokenType.FeatureDescription));
                    result.Add(new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan));
                }
            }
            return result;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleScenario(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleIt(span, evt, GherkinTokenType.ScenarioTitle);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleBackground(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleIt(span, evt, GherkinTokenType.BackgroundTitle);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleStep(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleIt(span, evt, GherkinTokenType.StepText);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTag(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleIt(span, evt, GherkinTokenType.Tag).ToList();
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleComment(SnapshotSpan span, GherkinParseEvent evt)
        {
            var text = span.GetText();
            var isLanguage = new Regex(@"\s*#\s*language\s*(:|\s)\s*(?<language>[a-zA-Z\-]+)");
            var match = isLanguage.Match(text);
            if (match.Success)
            {
                foreach (var commentTag in TagLanguageComment(span, evt, match, text))
                    yield return commentTag;
            }
            else
            {
                var r = new Regex(string.Format(@"^\s*#\s*{0}\s*$", evt.Tokens[0].Content));
                if (r.IsMatch(text))
                {
                    var t = new Token(text, evt.Tokens[0].LineInFile);
                    var tag = CreateTag(t, span, evt);
                    if (tag != null)
                        yield return tag;
                }
            }
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> TagLanguageComment(SnapshotSpan span, GherkinParseEvent evt, Match match, string text)
        {
            var i = text.IndexOf("language");
            var t1 = new Token(text.Substring(0, i), evt.Tokens[0].LineInFile);
            var comment = CreateTag(t1, span, evt);
            if (comment != null)
                yield return comment;
            var t2 = new Token(match.Value.Substring(i), evt.Tokens[0].LineInFile);
            var lang = CreateTag(t2, span, new GherkinParseEvent(GherkinTokenType.Tag, evt.Tokens.ToArray()));
            if (lang != null)
                yield return lang;
            var t3 = new Token(text.Substring(i + t2.Content.Length).TrimEnd(WhiteSpaces), evt.Tokens[0].LineInFile);
            if (t3.Content != "")
            {
                var end = CreateTag(t3, span, evt);
                if (end != null)
                    yield return end;
            }
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> SyntaxError(SnapshotSpan span, GherkinParseEvent evt)
        {
            var text = span.GetText();
            var token = new Token(text, new LineInFile(-1));
            var tag = CreateTag(token, span, GherkinTokenType.SyntaxError, text);
            if (tag != null)
                yield return tag;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleIt(SnapshotSpan span, GherkinParseEvent evt, GherkinTokenType titleType)
        {
            var token = evt.Tokens.First();
            var text = span.GetText();
            if (text.TrimStart(WhiteSpaces).StartsWith(token.Content, StringComparison.CurrentCultureIgnoreCase))
            {
                var tag = CreateTag(token, span, evt);
                if (tag != null)
                    yield return tag;
            }
            if (evt.Tokens.Count > 1)
            {
                token = evt.Tokens.Skip(1).First();
                var tag = CreateTag(token, span, titleType, text);
                if (tag != null)
                    yield return tag;
            }
        }

        private Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>> CreateTag(Token token, SnapshotSpan span, GherkinParseEvent evt)
        {
            var text = span.GetText();
            return CreateTag(token, span, evt.GherkinTokenType, text);
        }

        private Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>> CreateTag(Token token, SnapshotSpan span, GherkinTokenType tokenType, string text)
        {
            var idx = text.IndexOf(token.Content, StringComparison.Ordinal);
            if (idx == -1)
                return null;
            ITextSnapshotLine containingLine = span.Start.GetContainingLine();
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position + idx, token.Content.Length));
            var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(tokenType));
            return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);
        }
    }
}