using System.Text.RegularExpressions;
using gherkin;
using gherkin.lexer;

namespace NBehave.Narrator.Framework
{
    public class LanguageService
    {
        private const string DefaultLanguage = "en";
        private readonly char[] _whiteSpaceChars = new[] { ' ', '\t', '\n', '\r' };

        public Lexer GetLexer(string scenarioText, Listener gherkinScenarioParser)
        {
            I18n gherkinLanguageService = GetGherkinLanguageService(scenarioText);

            return gherkinLanguageService.lexer(gherkinScenarioParser);
        }

        private I18n GetGherkinLanguageService(string scenarioText)
        {
            var language = DefaultLanguage;
            var trimmed = scenarioText.TrimStart(_whiteSpaceChars);
            var lang = new Regex(@"^# language:\s+(?<language>\w+)\s+");
            var matches = lang.Match(trimmed);
            if (matches.Success)
            {
                language = matches.Groups["language"].Value;
            }

            return new I18n(language);
        }
    }
}