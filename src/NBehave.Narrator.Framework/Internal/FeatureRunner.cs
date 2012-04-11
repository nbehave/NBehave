using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework.Internal
{
    public class FeatureRunner : IFeatureRunner
    {
        private readonly IStringStepRunner stringStepRunner;
        private readonly IRunContext context;

        public FeatureRunner(IStringStepRunner stringStepRunner, IRunContext context)
        {
            this.context = context;
            this.stringStepRunner = stringStepRunner;
        }

        public FeatureResult Run(Feature feature)
        {
            var featureResult = new FeatureResult();
            foreach (var scenario in feature.Scenarios)
            {
                context.ScenarioStartedEvent(scenario);
                ScenarioResult scenarioResult = scenario.Examples.Any() ? RunExamples(scenario) : RunScenario(scenario);
                featureResult.AddResult(scenarioResult);
                context.ScenarioFinishedEvent(scenarioResult);
            }
            return featureResult;
        }

        private ScenarioResult RunScenario(Scenario scenario)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            BeforeScenario();
            var backgroundResults = RunBackground(scenario.Feature.Background);
            scenarioResult.AddActionStepResults(backgroundResults);
            var stepResults = RunSteps(scenario.Steps);
            scenarioResult.AddActionStepResults(stepResults);
            AfterScenario(scenario, scenarioResult);
            return scenarioResult;
        }

        private ScenarioResult RunExamples(Scenario scenario)
        {
            var runner = new ExampleRunner();
            var exampleResults = runner.RunExamples(scenario, RunSteps, BeforeScenario, AfterScenario);
            return exampleResults;
        }

        private IEnumerable<StepResult> RunBackground(Scenario background)
        {
            return RunSteps(background.Steps)
                .Select(_ => new BackgroundStepResult(background.Title, _))
                .Cast<StepResult>()
                .ToList();
        }

        private IEnumerable<StepResult> RunSteps(IEnumerable<StringStep> stepsToRun)
        {
            var failedStep = false;
            var stepResults = new List<StepResult>();
            foreach (var step in stepsToRun)
            {
                context.StepStarted(step);
                if (failedStep)
                {
                    step.PendBecauseOfPreviousFailedStep();
                    stepResults.Add(step.StepResult);
                    continue;
                }

                stringStepRunner.Run(step);

                if (step.StepResult.Result is Failed)
                {
                    failedStep = true;
                }
                stepResults.Add(step.StepResult);
                context.StepFinished(step.StepResult);
            }
            return stepResults;
        }

        private void BeforeScenario()
        {
            stringStepRunner.BeforeScenario();
        }

        private void AfterScenario(Scenario scenario, ScenarioResult scenarioResult)
        {
            if (scenario.Steps.Any())
            {
                try
                {
                    stringStepRunner.AfterScenario();
                }
                catch (Exception e)
                {
                    if (!scenarioResult.HasFailedSteps())
                        scenarioResult.Fail(new WrappedException(e));
                }
            }
        }
    }
}