using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class StoryResults : MarshalByRefObject
    {
        private readonly List<ScenarioResult> _scenarioResults = new List<ScenarioResult>();

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
                return _scenarioResults.FindAll(MatchByScenarioResult(typeof(Failed))).Count;
            }
        }

        public int NumberOfPendingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(MatchByScenarioResult(typeof(Pending))).Count;
            }
        }

        public int NumberOfPassingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(MatchByScenarioResult(typeof(Passed))).Count;
            }
        }

        public ScenarioResult[] ScenarioResults
        {
            get
            {
            	return _scenarioResults.ToArray();
            }
        }

        public void AddResult(ScenarioResult scenarioResult)
        {
            _scenarioResults.Add(scenarioResult);
        }

        private Predicate<ScenarioResult> MatchByScenarioResult(Type result)
        {
            return results => results.Result.GetType() == result;
        }
    }
}