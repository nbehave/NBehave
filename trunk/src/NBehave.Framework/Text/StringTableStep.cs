using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class StringTableStep : StringStep
    {
        private readonly List<Row> _tableSteps = new List<Row>();

        public StringTableStep(string step, string fromFile, IStringStepRunner stringTableStepRunner)
            : base(step, fromFile, stringTableStepRunner)
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
            bool hasParamsInStep = HasParametersInStep();
            foreach (Row row in _tableSteps)
            {
                StringStep step = this;
                if (hasParamsInStep)
                    step = InsertParametersToStep(row);
                ActionStepResult result = StringStepRunner.Run(step, row);
                actionStepResult.MergeResult(result);
            }
            return actionStepResult;
        }

        readonly Regex _hasParamsInStep = new Regex(@"\[\w+\]");

        private bool HasParametersInStep()
        {
            return _hasParamsInStep.IsMatch(Step);
        }

        private StringStep InsertParametersToStep(Row step)
        {
            string stringStep = Step;
            foreach (var column in step.ColumnValues)
            {
                var replceWithValue = new Regex(string.Format(@"\[{0}\]", column.Key), RegexOptions.IgnoreCase);
                stringStep = replceWithValue.Replace(stringStep, column.Value);
            }
            return new StringStep(stringStep, FromFile, StringStepRunner);
        }
    }
}