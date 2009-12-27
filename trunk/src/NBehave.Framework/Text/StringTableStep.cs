using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class StringTableStep : StringStep
    {
        private readonly List<Row> _tableSteps = new List<Row>();

        public StringTableStep(string step, string fromFile, StringStepRunner stringStepRunner)
            : base(step, fromFile, stringStepRunner)
        { }


        public IEnumerable<Row> TableSteps
        {
            get
            {
                return _tableSteps;
            }
        }

        public void AddTableStep(Row row)
        {
            _tableSteps.Add(row);
        }

        public override ActionStepResult Run()
        {
            var actionStepResult = new ActionStepResult(Step, new Passed());
            foreach (var tableStep in _tableSteps)
            {
                ActionStepResult result = StringStepRunner.Run(this, tableStep);
                RaiseScenarioMessage(result);
                actionStepResult.MergeResult(result);
            }
            return actionStepResult;
        }
    }
}