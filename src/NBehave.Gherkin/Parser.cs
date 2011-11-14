using System.IO;
using GurkBurk;
//using gherkin.lexer;

namespace NBehave.Gherkin
{
    public class Parser
    {
        private readonly IListener _listener;

        public Parser(IListener listener)
        {
            _listener = listener;
        }

        public void Scan(string source)
        {
            var stream = CreateStream(source);
            Scan(stream);
        }

        public void Scan(TextReader stream)
        {
            var feature = stream.ReadToEnd();
            var lexer = new I18nLexer(new GherkinListener(_listener));
            lexer.scan(feature);
        }

        private TextReader CreateStream(string source)
        {
            var ms = new MemoryStream();
            var sr = new StreamWriter(ms);
            sr.Write(source.Replace("\r", string.Empty));
            sr.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return new StreamReader(ms);
        }
    }
}