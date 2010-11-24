using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class ScenarioExampleResult : ScenarioResult
    {
        private readonly List<ScenarioResult> _exampleRowResult = new List<ScenarioResult>();

        public ScenarioExampleResult(Feature feature, string scenarioTitle, IEnumerable<StringStep> stringSteps, IEnumerable<Example> examples)
            : base(feature, scenarioTitle)
        {
            AddSteps(stringSteps);
            Examples = examples;
        }

        private void AddSteps(IEnumerable<StringStep> stringSteps)
        {
            foreach (var stringStep in stringSteps)
            {
                AddActionStepResult(new ActionStepResult(stringStep.Step, new Passed()));
            }
        }

        public override void AddActionStepResult(ActionStepResult actionStepResult)
        {
            MergeResult(actionStepResult);
            var step = ActionStepResults.FirstOrDefault(s => s.StringStep == actionStepResult.StringStep);
            if (step == null)
                base.AddActionStepResult(actionStepResult);
            else
                step.MergeResult(actionStepResult);
        }

        public IEnumerable<ScenarioResult> ExampleResults { get { return _exampleRowResult; } }

        public IEnumerable<Example> Examples { get; private set; }

        public void AddResult(ScenarioResult exampleResult)
        {
            _exampleRowResult.Add(exampleResult);
            UpdateStepResults(exampleResult);
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