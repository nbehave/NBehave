using System;
using System.Linq;
using System.Text;
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
            var actionStepResult = GetNewActionStepResult();
            bool hasParamsInStep = HasParametersInStep();
            foreach (Row row in _tableSteps)
            {
                StringStep step = this;
                if (hasParamsInStep)
                    step = InsertParametersToStep(row);
                ActionStepResult result = StringStepRunner.Run(step, row);
                actionStepResult.MergeResult(result.Result);
            }
            return actionStepResult;
        }

        private ActionStepResult GetNewActionStepResult()
        {
            string fullStep = CreateStepText();
            return new ActionStepResult(fullStep, new Passed());
        }

        private string CreateStepText()
        {
            var step = new StringBuilder(Step + Environment.NewLine);
            step.Append(_tableSteps.First().ColumnNamesToString() + Environment.NewLine);
            foreach (var row in _tableSteps)
                step.Append(row.ColumnValuesToString() + Environment.NewLine);
            RemoveLastNewLine(step);
            return step.ToString();
        }

        private void RemoveLastNewLine(StringBuilder step)
        {
            step.Remove(step.Length - Environment.NewLine.Length, Environment.NewLine.Length);
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
            return new StringStep(stringStep, Source, StringStepRunner);
        }
    }
}