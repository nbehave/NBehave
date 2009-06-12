using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class StoryResults
    {
        private readonly List<ScenarioResults> _scenarioResults = new List<ScenarioResults>();

        public int NumberOfThemes { get; set; }

        public int NumberOfStories { get; set; }

        public int NumberOfScenariosFound
        {
            get { return _scenarioResults.Count; }
        }

        public int NumberOfFailingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(MatchByScenarioResult(ScenarioResult.Failed)).Count;
            }
        }

        public int NumberOfPendingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(MatchByScenarioResult(ScenarioResult.Pending)).Count;
            }
        }

        public int NumberOfPassingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(MatchByScenarioResult(ScenarioResult.Passed)).Count;
            }
        }

        public ScenarioResults[] ScenarioResults
        {
            get
            {
            	return _scenarioResults.ToArray();
            }
        }

        public void AddResult(ScenarioResults scenarioResults)
        {
            _scenarioResults.Add(scenarioResults);
        }

        private Predicate<ScenarioResults> MatchByScenarioResult(ScenarioResult result)
        {
            return results => results.ScenarioResult == result;
        }
    }
}