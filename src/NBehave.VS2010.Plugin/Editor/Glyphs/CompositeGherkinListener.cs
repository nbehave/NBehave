using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    public class CompositeGherkinListener : IListener
    {
        private readonly List<IListener> listeners;

        public CompositeGherkinListener(params IListener[] listeners)
        {
            this.listeners = listeners.ToList();
        }

        public void Feature(Token keyword, Token title, Token narrative)
        {
            listeners.ForEach(_ => _.Feature(keyword, title, narrative));
        }

        public void Scenario(Token keyword, Token title)
        {
            listeners.ForEach(_ => _.Scenario(keyword, title));
        }

        public void Examples(Token keyword, Token name)
        {
            listeners.ForEach(_ => _.Examples(keyword, name));
        }

        public void Step(Token keyword, Token name)
        {
            listeners.ForEach(_ => _.Step(keyword, name));
        }

        public void Table(IList<IList<Token>> rows, LineInFile lineInFile)
        {
            listeners.ForEach(_ => _.Table(rows, lineInFile));
        }

        public void Background(Token keyword, Token name)
        {
            listeners.ForEach(_ => _.Background(keyword, name));
        }

        public void Comment(Token comment)
        {
            listeners.ForEach(_ => _.Comment(comment));
        }

        public void Tag(Token name)
        {
            listeners.ForEach(_ => _.Tag(name));
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile)
        {
            listeners.ForEach(_ => _.SyntaxError(state, @event, legalEvents, lineInFile));
        }

        public void Eof()
        {
            listeners.ForEach(_ => _.Eof());
        }

        public void DocString(Token docString)
        {
            listeners.ForEach(_ => _.DocString(docString));
        }
    }
}