using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Domain;

namespace NBehave.EventListeners
{
    public class FailSpecResultEventListener : EventListener
    {
        private readonly List<ScenarioResult> _results = new List<ScenarioResult>();

        public override void RunFinished()
        {
            var failedScenarios = _results.Where(_ => _.Result is Failed).ToList();
            if (failedScenarios.Any() == false)
                return;
            var errors = new StringBuilder();
            foreach (var failedScenario in failedScenarios)
            {
                errors.Append(string.Format("Scenario {0} failed", failedScenario.ScenarioTitle));
                errors.Append(Environment.NewLine);
                foreach (var error in failedScenario.StepResults.Where(_ => _.Result is Failed))
                {
                    errors.AppendLine(string.Format("Step '{0}' failed with result:", error.StringStep));
                    errors.AppendLine(error.Result.Message);
                    errors.Append(Environment.NewLine);
                }
                errors.Append(Environment.NewLine);
            }
            throw new StepFailedException(errors.ToString());
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            _results.Add(result);
        }
    }

    public class StepFailedException : Exception
    {
        public StepFailedException(string message)
            : base(message)
        {

        }
    }

}