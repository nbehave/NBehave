using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class TokenStringsToScenarioParser
    {
        public TokenStringsToScenarioParser()
        {
            Scenarios = new List<string>();
        }

        public List<string> Scenarios { get; private set; }

        public void ParseTokensToScenarios(IList<string> tokens)
        {
            var firstToken = tokens[0];
            var firstWord = FirstWord(firstToken);
            var tokensInScenario = new List<string> { firstToken };
            var same = false;
            for (int i = 1; i < tokens.Count; i++)
            {
                if (firstWord.Equals(FirstWord(tokens[i]), StringComparison.CurrentCultureIgnoreCase) && same == false)
                {
                    Scenarios.Add(RowsToString(tokensInScenario));
                    tokensInScenario = new List<string> { tokens[i] };
                    same = true;
                }
                else
                {
                    same = false;
                    tokensInScenario.Add(tokens[i]);
                }
            }
            if (tokensInScenario.Count > 0)
                Scenarios.Add(RowsToString(tokensInScenario));
        }

        private string RowsToString(IEnumerable<string> rows)
        {
            string text = string.Empty;
            foreach (var row in rows)
            {
                text += row + Environment.NewLine;
            }
            return text.TrimEnd(Environment.NewLine.ToCharArray());
        }

        private string FirstWord(string text)
        {
            var match = new Regex(@"\w+");
            return match.Match(text).Value;
        }
    }
}