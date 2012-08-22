using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.LanguageService
{
    public class GherkinParseEvent
    {
        public GherkinParseEvent(GherkinTokenType gherkinTokenType, params Token[] tokens)
        {
            GherkinTokenType = gherkinTokenType;
            Tokens = tokens.ToList();
        }

        public readonly GherkinTokenType GherkinTokenType;
        public readonly List<Token> Tokens;
    }
}