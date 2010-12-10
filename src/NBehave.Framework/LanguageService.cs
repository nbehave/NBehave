// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the LanguageService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Text.RegularExpressions;

    using Gherkin;

    public class LanguageService
    {
        private const string DefaultLanguage = "en";
        private readonly char[] _whiteSpaceChars = new[] { ' ', '\t', '\n', '\r' };

        public ILexer GetLexer(string scenarioText, IListener gherkinScenarioParser)
        {
            var gherkinLanguage = GetGherkinLanguage(scenarioText);
            return Lexers.Create(gherkinLanguage, gherkinScenarioParser);
        }

        private string GetGherkinLanguage(string scenarioText)
        {
            var language = DefaultLanguage;
            var trimmed = scenarioText.TrimStart(_whiteSpaceChars);
            var lang = new Regex(@"^# language:\s+(?<language>\w+)\s+");
            var matches = lang.Match(trimmed);
            if (matches.Success)
            {
                language = matches.Groups["language"].Value;
            }

            return language;
        }
    }
}