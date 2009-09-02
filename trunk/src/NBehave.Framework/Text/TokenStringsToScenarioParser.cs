using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class TokenStringsToScenarioParser
    {
        private readonly ActionStep _actionStep;

        public TokenStringsToScenarioParser(ActionStep actionStep)
        {
            _actionStep = actionStep;
            Scenarios = new List<ScenarioSteps>();
        }

        public List<ScenarioSteps> Scenarios { get; private set; }

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
                        var scenarioSteps = new ScenarioSteps { Steps = RowsToString(tokensInScenario) };
                        Scenarios.Add(scenarioSteps);
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
            {
                var scenarioSteps = new ScenarioSteps { Steps = RowsToString(tokensInScenario) };
                Scenarios.Add(scenarioSteps);
            }
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