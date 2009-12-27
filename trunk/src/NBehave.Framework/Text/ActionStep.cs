using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionStep
    {
        public static readonly IEnumerable<string> FeatureSteps = new[] { "Story", "Feature" };
        public const string ScenarioTitle = "Scenario";
        public static readonly IEnumerable<string> ScenarioSteps = new[] { ScenarioTitle, Examples, "Given", "When", "Then", "And" };
        public const string Examples = "Examples";


        public bool IsFeatureTitle(string actionStep)
        {
            return (IsStepWithMoreThanOneKeyWord(actionStep, FeatureSteps));
        }

        private bool IsStepWithMoreThanOneKeyWord(string actionStep, IEnumerable<string> keyWords)
        {
            foreach (var action in keyWords)
            {
                var isNarrative = new Regex(string.Format(@"\s*{0}:?\s+", action));
                if (isNarrative.IsMatch(actionStep))
                    return true;
            }
            return false;
        }

        private readonly Regex _title = new Regex(@"\s*\w+:?\s+(?<title>.*)", RegexOptions.Compiled);
        public string GetTitle(string actionStep)
        {
            Match m = _title.Match(actionStep);
            string title = m.Groups["title"].Value;
            return title;
        }

        private readonly Regex _isScenarioTitle = new Regex(string.Format(@"\s*{0}:?\s+", ScenarioTitle), RegexOptions.Compiled);
        public bool IsScenarioTitle(string text)
        {
            return _isScenarioTitle.IsMatch(text);
        }

        public bool IsScenarioStep(string text)
        {
            foreach (var step in ScenarioSteps)
            {
                var isScenarioStep = new Regex(string.Format(@"\s*{0}\s+", step));
                if (isScenarioStep.IsMatch(text))
                    return true;
            }

            return IsScenarioTitle(text);
        }

        private readonly Regex _isExample = new Regex(string.Format(@"\s*{0}:?\s+", Examples), RegexOptions.Compiled);
        public bool IsExample(string text)
        {
            return _isExample.IsMatch(text);
            
        }
    }
}
