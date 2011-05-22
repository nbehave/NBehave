// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioExampleResult.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioExampleResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;
    using System.Linq;

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

        public override void AddActionStepResult(ActionStepResult actionStepResult)
        {
            MergeResult(actionStepResult);
            var step = ActionStepResults.FirstOrDefault(s => s.StringStep == actionStepResult.StringStep);
            if (step == null)
            {
                base.AddActionStepResult(actionStepResult);
            }
            else
            {
                step.MergeResult(actionStepResult);
            }
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
                AddActionStepResult(new ActionStepResult(stringStep.Step, new Passed()));
            }
        }

        private void UpdateStepResults(ScenarioResult result)
        {
            var actionStepResults = ActionStepResults.ToArray();

            var idx = 0;
            foreach (var stepResult in result.ActionStepResults)
            {
                var step = actionStepResults[idx++];
                step.MergeResult(stepResult.Result);
                MergeResult(stepResult);
            }
        }
    }
}