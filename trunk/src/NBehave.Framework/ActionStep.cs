using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionStep
    {
        public const string StoryTitle = "Story";
        public static readonly IEnumerable<string> StoryNarrative = new[] { "As a", "I want", "So that" };
        public static readonly IEnumerable<string> StorySteps = new[] { StoryTitle, "As a", "I want", "So that" };
        public const string ScenarioTitle = "Scenario";
        public static readonly IEnumerable<string> ScenarioSteps = new[] { ScenarioTitle, "Given", "When", "Then" };

        private ActionStepAlias _actionStepAlias;

        public ActionStep(ActionStepAlias actionStepAlias)
        {
            _actionStepAlias = actionStepAlias;
        }

        private readonly Regex _isStoryTitle = new Regex(string.Format(@"\s*{0}:?\s+", StoryTitle), RegexOptions.Compiled);
        public bool IsStoryTitle(string actionStep)
        {
            return _isStoryTitle.IsMatch(actionStep);
        }

        public bool IsNarrative(string actionStep)
        {
            foreach (var action in StoryNarrative)
            {
                Regex isNarrative = new Regex(string.Format(@"\s*{0}:?\s+", action));
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

        private readonly Regex _isScenarioStep = new Regex(string.Format(@"\s*{0}:?\s+", "Given"), RegexOptions.Compiled);
        public bool IsScenarioStep(string text)
        {
            foreach (var step in ScenarioSteps)
            {
                Regex isScenarioStep = new Regex(string.Format(@"\s*{0}\s+", step));
                if (isScenarioStep.IsMatch(text))
                    return true;
            }

            return IsScenarioTitle(text);
        }
    }
}
