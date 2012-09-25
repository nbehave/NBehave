using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.Tagging
{
    public static class WhiteSpaces
    {
        public static readonly char[] Chars = new[] { '\t', ' ', '\r', '\n' };
    }
    public class GherkinStepTagger
    {
        private readonly Dictionary<GherkinTokenType, Func<SnapshotSpan, GherkinParseEvent, IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>>>> tagHandler;
        private bool cellToggle;

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
                    {GherkinTokenType.DocString, HandleDocString},
                    {GherkinTokenType.Examples, HandleSpan},
                    {GherkinTokenType.Step, HandleStep},
                    {GherkinTokenType.TableHeader, HandleTableHeader},
                    {GherkinTokenType.TableCell, HandleTableCell},
                };
        }

        public IEnumerable<ITagSpan<GherkinTokenTag>> CreateTags(IEnumerable<GherkinParseEvent> events, SnapshotSpan span)
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

        private GherkinParseEvent FindMatchingEvent(IEnumerable<GherkinParseEvent> events, int lineNumber)
        {
            var evt = events.FirstOrDefault(_ => _.Tokens.Any(λ => λ.LineInFile.Line == lineNumber))
                      ?? events.Where(_ => _.Tokens.Any(λ => λ.LineInFile.Line < lineNumber))
                             .OrderByDescending(_ => _.Tokens.First().LineInFile.Line)
                             .FirstOrDefault();
            if (evt != null && evt.GherkinTokenType == GherkinTokenType.SyntaxError && lineNumber > evt.Tokens.Max(_ => _.LineInFile.Line))
                return null;
            return evt;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleSpan(SnapshotSpan span, GherkinParseEvent evt)
        {
            ITextSnapshotLine containingLine = span.Start.GetContainingLine();
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position, containingLine.Length));
            var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(evt.GherkinTokenType));
            yield return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleFeature(SnapshotSpan span, GherkinParseEvent evt)
        {
            if (!evt.Tokens.Any())
                yield break;
            var t1 = HandleType(span, evt.Tokens.First(), GherkinTokenType.Feature);
            if (t1 != null) yield return t1;
            if (evt.Tokens.Count >= 2)
            {
                var t2 = HandleTitle(span, evt.Tokens[1], GherkinTokenType.FeatureTitle);
                foreach (var tuple in t2)
                    yield return tuple;
            }
            if (evt.Tokens.Count == 3)
            {
                var t3 = HandleTitle(span, evt.Tokens[2], GherkinTokenType.FeatureDescription);
                foreach (var tuple in t3)
                    yield return tuple;
            }
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleScenario(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleTag(span, evt, GherkinTokenType.Scenario, GherkinTokenType.ScenarioTitle);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleBackground(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleTag(span, evt, GherkinTokenType.Background, GherkinTokenType.BackgroundTitle);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleStep(SnapshotSpan span, GherkinParseEvent evt)
        {
            return HandleTag(span, evt, GherkinTokenType.Step, GherkinTokenType.StepText);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTag(SnapshotSpan span, GherkinParseEvent evt)
        {
            var tags = new List<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>>();
            foreach (var token in evt.Tokens)
                tags.AddRange(HandleTitle(span, token, GherkinTokenType.Tag));
            return tags;
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
            var t3 = new Token(text.Substring(i + t2.Content.Length).TrimEnd(WhiteSpaces.Chars), evt.Tokens[0].LineInFile);
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

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTableHeader(SnapshotSpan span, GherkinParseEvent evt)
        {
            cellToggle = false;
            return TableItem(span, evt.GherkinTokenType);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTableCell(SnapshotSpan span, GherkinParseEvent evt)
        {
            var tokenType = cellToggle ? GherkinTokenType.TableCell : GherkinTokenType.TableCellAlt;
            cellToggle = !cellToggle;
            return TableItem(span, tokenType);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> TableItem(SnapshotSpan span, GherkinTokenType tokenType)
        {
            return HandleText(span, tokenType, "|");
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleDocString(SnapshotSpan span, GherkinParseEvent evt)
        {
            var text = span.GetText().Trim(WhiteSpaces.Chars);
            if (text == "\"\"\"")
                return HandleText(span, GherkinTokenType.Tag, "\"");

            //TODO: dont color the spaces that will be removed
            ITextSnapshotLine containingLine = span.Start.GetContainingLine();
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position, containingLine.Length));
            var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(evt.GherkinTokenType));
            return new[] { new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan) };
        }

        private static IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleText(
            SnapshotSpan span, GherkinTokenType tokenType,
            string surroundingText)
        {
            var str = span.GetText().TrimEnd(WhiteSpaces.Chars);
            var idx = str.IndexOf(surroundingText, StringComparison.InvariantCulture);
            var idx2 = str.LastIndexOf(surroundingText, StringComparison.InvariantCulture);
            if (idx == -1 || idx2 == -1)
                yield break;
            ITextSnapshotLine containingLine = span.Start.GetContainingLine();
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position + idx, idx2 - idx + 1));
            var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(tokenType));
            yield return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTag(SnapshotSpan span, GherkinParseEvent evt, GherkinTokenType tokenType, GherkinTokenType tokenTitleType)
        {
            if (!evt.Tokens.Any())
                yield break;
            var t1 = HandleType(span, evt.Tokens.First(), tokenType);
            if (t1 != null) yield return t1;
            if (evt.Tokens.Count >= 2)
            {
                var t2 = HandleTitle(span, evt.Tokens[1], tokenTitleType);
                foreach (var tuple in t2)
                    yield return tuple;
            }
        }

        private Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>> HandleType(SnapshotSpan span, Token token, GherkinTokenType tokenType)
        {
            var text = span.GetText();
            if (text.TrimStart(WhiteSpaces.Chars).StartsWith(token.Content))
            {
                var idx = text.IndexOf(token.Content, StringComparison.Ordinal);
                ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position + idx, token.Content.Length));
                var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(tokenType));
                return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);
            }
            return null;
        }

        private IEnumerable<Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>> HandleTitle(SnapshotSpan span, Token token, GherkinTokenType tokenType)
        {
            var spanText = span.GetText();
            var tokenText = token.Content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim(WhiteSpaces.Chars)).ToList();
            foreach (var row in tokenText)
            {
                var idx = spanText.IndexOf(row, StringComparison.CurrentCulture);
                if (idx == -1)
                    continue;
                ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position + idx, row.Length));
                var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(tokenType));
                yield return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);

            }
        }

        private Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>> CreateTag(Token token, SnapshotSpan span, GherkinParseEvent evt)
        {
            var text = span.GetText();
            return CreateTag(token, span, evt.GherkinTokenType, text);
        }

        private Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>> CreateTag(Token token, SnapshotSpan span, GherkinTokenType tokenType, string text)
        {
            foreach (var row in token.Content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string rowText = row.Trim(WhiteSpaces.Chars);
                var idx = text.IndexOf(rowText, StringComparison.Ordinal);
                if (idx == -1)
                    continue;
                ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(containingLine.Start.Position + idx, rowText.Length));
                var tagSpan = new TagSpan<GherkinTokenTag>(tokenSpan, new GherkinTokenTag(tokenType));
                return new Tuple<SnapshotSpan, TagSpan<GherkinTokenTag>>(tokenSpan, tagSpan);
            }
            return null;
        }
    }
}