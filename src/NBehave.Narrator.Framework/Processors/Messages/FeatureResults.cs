// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureResults.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the FeatureResults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;

    using NBehave.Narrator.Framework.Tiny;

    [Serializable]
    public class FeatureResults : TinyMessageBase
    {
        private readonly List<ScenarioResult> _scenarioResults = new List<ScenarioResult>();

        public FeatureResults(object sender) : base(sender)
        {
        }

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