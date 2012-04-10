using System.Collections.Generic;

namespace NBehave.Gherkin
{
    public interface IListener
    {
        void Feature(Token keyword, Token title, Token narrative);
        void Scenario(Token keyword, Token title);
        void Examples(Token keyword, Token name);
        void Step(Token keyword, Token name);
        void Table(IList<IList<Token>> rows, LineInFile tableRow);
        void Background(Token keyword, Token name);
        void Comment(Token comment);
        void Tag(Token name);
        void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile);
        void Eof();
        void DocString(Token docString);
    }
}
