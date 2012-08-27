using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;

namespace NBehave.VS2010.Plugin.Tagging
{
    [Export(typeof(TokenParserFactory))]
    public class TokenParserFactory
    {
        public TokenParser Build(ITextBuffer buffer)
        {
            TokenParser parser;
            if (buffer.Properties.TryGetProperty(typeof(TokenParser), out parser))
                return parser;
            parser = new TokenParser(buffer);
            buffer.Properties.AddProperty(typeof(TokenParser), parser);
            return parser;
        }
    }
}