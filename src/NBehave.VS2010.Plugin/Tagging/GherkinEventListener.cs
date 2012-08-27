using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.Tagging
{
    public class GherkinEventListener : IListener
    {
        private readonly List<GherkinParseEvent> events = new List<GherkinParseEvent>();

        public List<GherkinParseEvent> Events
        {
            get { return events; }
        }

        public void Feature(Token keyword, Token title, Token narrative)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Feature, keyword, title, narrative));
        }

        public void Scenario(Token keyword, Token title)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Scenario, keyword, title));
        }

        public void Examples(Token keyword, Token name)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Examples, keyword, name));
        }

        public void Step(Token keyword, Token name)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Step, keyword, name));
        }

        public void Table(IList<IList<Token>> rows, LineInFile lineInFile)
        {
            if (rows.Any())
            {
                var header = rows.First();
                events.Add(new GherkinParseEvent(GherkinTokenType.TableHeader, new Token(string.Join(" | ", header).Trim(), lineInFile)));
                var cells = rows.Skip(1);
                var cellLine = lineInFile;
                foreach (var cell in cells)
                {
                    cellLine = new LineInFile(cellLine.Line + 1);
                    events.Add(new GherkinParseEvent(GherkinTokenType.TableCell, new Token(string.Join(" | ", cell).Trim(), cellLine)));
                }
            }
        }

        public void Background(Token keyword, Token name)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Background, keyword, name));
        }

        public void Comment(Token comment)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Comment, comment));
        }

        public void Tag(Token tag)
        {
            if (tag.Content != "@") //bug in gherkin parser?
            {
                var previous = events.LastOrDefault() ?? new GherkinParseEvent(GherkinTokenType.SyntaxError);
                if (previous.GherkinTokenType == GherkinTokenType.Tag && tag.LineInFile.Line == previous.Tokens[0].LineInFile.Line)
                    previous.Tokens.Add(tag);
                else
                    events.Add(new GherkinParseEvent(GherkinTokenType.Tag, tag));
            }
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.SyntaxError, new Token(@event, lineInFile)));
        }

        public void Eof()
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.Eof));
        }

        public void DocString(Token docString)
        {
            events.Add(new GherkinParseEvent(GherkinTokenType.DocString, docString));
        }
    }
}