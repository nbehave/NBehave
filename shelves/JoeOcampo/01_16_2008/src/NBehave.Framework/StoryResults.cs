using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class StoryResults
    {
        private int _numberOfThemes;
        private int _numberOfStories;
        private List<ScenarioResults> _scenarioResults = new List<ScenarioResults>();

        public int NumberOfThemes
        {
            get { return _numberOfThemes; }
            set { _numberOfThemes = value; }
        }

        public int NumberOfStories
        {
            get { return _numberOfStories; }
            set { _numberOfStories = value; }
        }

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
            return delegate(ScenarioResults results) { return results.ScenarioResult == result; };
        }
    }
}