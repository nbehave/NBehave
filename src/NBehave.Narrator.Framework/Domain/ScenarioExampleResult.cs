// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioExampleResult.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioExampleResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class ScenarioExampleResult : ScenarioResult
    {
        private readonly List<ScenarioResult> _exampleRowResult = new List<ScenarioResult>();

        public ScenarioExampleResult(Feature feature, string scenarioTitle, IEnumerable<StringStep> stringSteps, IEnumerable<Example> examples)
            : base(feature, scenarioTitle)
        {
            AddSteps(stringSteps);
            Examples = examples;
        }

        public IEnumerable<ScenarioResult> ExampleResults
        {
            get
            {
                return _exampleRowResult;
            }
        }

        public IEnumerable<Example> Examples { get; private set; }

        public override void AddActionStepResult(StepResult stepResult)
        {
            MergeResult(stepResult);
            var step = StepResults.FirstOrDefault(s => s.StringStep == stepResult.StringStep);
            if (step == null)
                base.AddActionStepResult(stepResult);
            else
                MergeStepResult(stepResult, step);
        }

        public void AddResult(ScenarioResult exampleResult)
        {
            _exampleRowResult.Add(exampleResult);
            UpdateStepResults(exampleResult);
        }

        private void AddSteps(IEnumerable<StringStep> stringSteps)
        {
            foreach (var stringStep in stringSteps)
            {
                AddActionStepResult(new StepResult(stringStep, new Passed()));
            }
        }

        private void UpdateStepResults(ScenarioResult result)
        {
            var actionStepResults = StepResults.ToArray();

            var idx = 0;
            foreach (var stepResult in result.StepResults)
            {
                var step = actionStepResults[idx++];
                MergeStepResult(stepResult, step);
                MergeResult(stepResult);
            }
        }

        private static void MergeStepResult(StepResult stepResult, StepResult steptoMergeInto)
        {
            steptoMergeInto.MergeResult(stepResult.Result);
        }
    }
}