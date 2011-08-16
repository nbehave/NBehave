using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework.Processors
{
    public class StringTableStepRunner
    {
        private readonly Regex _hasParamsInStep = new Regex(@"\[\w+\]");
        private readonly IStringStepRunner _stringStepRunner;

        public StringTableStepRunner(IStringStepRunner stringStepRunner)
        {
            _stringStepRunner = stringStepRunner;
        }

        public void RunStringTableStep(StringTableStep stringStep)
        {
            var actionStepResult = GetNewActionStepResult(stringStep);
            var hasParamsInStep = HasParametersInStep(stringStep.Step);
            foreach (var row in stringStep.TableSteps)
            {
                StringStep step = stringStep;
                if (hasParamsInStep)
                {
                    step = InsertParametersToStep(stringStep, row);
                }

                var result = _stringStepRunner.Run(step, row);
                actionStepResult.MergeResult(result.Result);
            }

            stringStep.StepResult = actionStepResult;
        }

        private StepResult GetNewActionStepResult(StringTableStep stringStep)
        {
            var fullStep = new ActionStepText(CreateStepText(stringStep), stringStep.Source);
            return new StepResult(fullStep, new Passed());
        }

        private string CreateStepText(StringTableStep stringStep)
        {
            var step = new StringBuilder(stringStep.Step + Environment.NewLine);
            step.Append(stringStep.TableSteps.First().ColumnNamesToString() + Environment.NewLine);
            foreach (var row in stringStep.TableSteps)
            {
                step.Append(row.ColumnValuesToString() + Environment.NewLine);
            }

            RemoveLastNewLine(step);
            return step.ToString();
        }

        private void RemoveLastNewLine(StringBuilder step)
        {
            step.Remove(step.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

        private bool HasParametersInStep(string step)
        {
            return _hasParamsInStep.IsMatch(step);
        }

        private StringStep InsertParametersToStep(StringTableStep step, Row row)
        {
            var stringStep = step.Step;
            foreach (var column in row.ColumnValues)
            {
                var replceWithValue = new Regex(string.Format(@"\[{0}\]", column.Key), RegexOptions.IgnoreCase);
                stringStep = replceWithValue.Replace(stringStep, column.Value);
            }
            return new StringStep(stringStep, step.Source);
        }

    }
}