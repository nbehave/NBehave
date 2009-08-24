using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class TokenStringsToScenarioParser
    {
        private ActionStep _actionStep;

        public TokenStringsToScenarioParser(ActionStep actionStep)
        {
            _actionStep = actionStep;
            Scenarios = new List<string>();
        }

        public List<string> Scenarios { get; private set; }

        public void ParseTokensToScenarios(IList<string> actionSteps)
        {
            var tokensInScenario = new List<string>();
            bool isFirst = true;
            bool isFirstTokenWordOfScenario = false;
            string firstWord = "_";
            for (int i = 0; i < actionSteps.Count; i++)
            {
                string actionStep = actionSteps[i];
                bool isStoryTitle = _actionStep.IsStoryTitle(actionStep);
                bool isNarrative = _actionStep.IsNarrative(actionStep);
                bool isScenarioTitle = _actionStep.IsScenarioTitle(actionStep);
                bool isScenarioWord = _actionStep.IsScenarioStep(actionStep);

                if (isFirst && (isStoryTitle || isNarrative || isScenarioTitle || isScenarioWord))
                {
                    tokensInScenario.Add(actionStep);
                    if (isScenarioWord || isScenarioTitle)
                    {
                        isFirst = false;
                        firstWord = actionStep.GetFirstWord();
                    }
                }
                else
                {
                    if ( isScenarioTitle ||
                        (firstWord.Equals(actionStep.GetFirstWord(), StringComparison.CurrentCultureIgnoreCase) && isFirstTokenWordOfScenario == false))
                    {
                        Scenarios.Add(RowsToString(tokensInScenario));
                        tokensInScenario = new List<string> { actionStep };
                        isFirstTokenWordOfScenario = true;
                    }
                    else
                    {
                        isFirstTokenWordOfScenario = false;
                        tokensInScenario.Add(actionStep);
                    }
                }

            }
            if (tokensInScenario.Count > 0)
                Scenarios.Add(RowsToString(tokensInScenario));
        }

        public void OldParseTokensToScenarios(IList<string> tokens)
        {
            var firstToken = tokens[0];
            var firstWord = firstToken.GetFirstWord();
            var tokensInScenario = new List<string> { firstToken };
            var same = false;
            for (int i = 1; i < tokens.Count; i++)
            {
                if (firstWord.Equals(tokens[i].GetFirstWord(), StringComparison.CurrentCultureIgnoreCase)
                    && same == false)
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
    }
}