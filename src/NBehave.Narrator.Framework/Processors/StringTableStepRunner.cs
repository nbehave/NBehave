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

        public StepResult RunStringTableStep(StringTableStep step)
        {
            var stringStep = GetNewStringStep(step);
            foreach (var example in step.TableSteps)
            {
                var stepWithParameters = AddParametersToStep(step, example);

                _stringStepRunner.Run(stepWithParameters, example);
                stringStep.StepResult.MergeResult(stepWithParameters.StepResult.Result);
            }

            return stringStep.StepResult;
        }

        private StringStep GetNewStringStep(StringTableStep stringStep)
        {
            return new StringStep(CreateStepText(stringStep), stringStep.Source);
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

        private StringStep AddParametersToStep(StringTableStep step, Example row)
        {
            if (HasParametersInStep(step.Step) == false)
                return new StringStep(step.Step, step.Source);

            var stringStep = step.Step;
            foreach (var column in row.ColumnValues)
            {
                var replceWithValue = new Regex(string.Format(@"\[{0}\]", column.Key), RegexOptions.IgnoreCase);
                stringStep = replceWithValue.Replace(stringStep, column.Value);
            }
            return new StringStep(stringStep, step.Source);
        }

        private bool HasParametersInStep(string step)
        {
            return _hasParamsInStep.IsMatch(step);
        }
    }
}