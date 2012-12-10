using System;
using System.Linq;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class FeatureResults : List<FeatureResult>
    {
        public int NumberOfScenariosFound
        {
            get { return this.SelectMany(_ => _.ScenarioResults).Count(); }
        }

        public int NumberOfFailingScenarios
        {
            get
            {
                return this.SelectMany(_ => _.ScenarioResults).Count(_ => _.Result is Failed);
            }
        }

        public int NumberOfPendingScenarios
        {
            get
            {
                return this.SelectMany(_ => _.ScenarioResults).Count(_ => _.Result is Pending);
            }
        }

        public int NumberOfPassingScenarios
        {
            get
            {
                return this.SelectMany(_ => _.ScenarioResults).Count(_ => _.Result is Passed);
            }
        }
    }

    [Serializable]
    public class FeatureResult
    {
        private readonly List<ScenarioResult> _scenarioResults = new List<ScenarioResult>();

        public FeatureResult(string featureTitle)
        {
            FeatureTitle = featureTitle;
        }

        public string FeatureTitle { get; private set; }

        public int NumberOfScenariosFound
        {
            get { return _scenarioResults.Count; }
        }

        public int NumberOfFailingScenarios
        {
            get
            {
                return _scenarioResults.Count(_ => _.Result is Failed);
            }
        }

        public int NumberOfPendingScenarios
        {
            get
            {
                return _scenarioResults.FindAll(_ => _.Result is Pending).Count;
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
    }
}