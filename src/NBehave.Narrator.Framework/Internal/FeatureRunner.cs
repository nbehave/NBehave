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
                var backgroundResults = RunBackground(scenario.Feature.Background);
                context.ScenarioStartedEvent(scenario);
                ScenarioResult scenarioResult = scenario.Examples.Any() ? RunExamples(scenario) : RunScenario(scenario);
                scenarioResult.AddActionStepResults(backgroundResults);
                featureResult.AddResult(scenarioResult);
                context.ScenarioFinishedEvent(scenarioResult);
            }
            return featureResult;
        }

        private ScenarioResult RunScenario(Scenario scenario)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            BeforeScenario();
            var stepResults = RunSteps(scenario.Steps, BeforeStep, AfterStep);

            scenarioResult.AddActionStepResults(stepResults);
            AfterScenario(scenario, scenarioResult);
            return scenarioResult;
        }

        private void AfterStep(StringStep step)
        {
            context.StepFinished(step.StepResult);
        }

        private void BeforeStep(StringStep step)
        {
            context.StepStarted(step);
        }

        private ScenarioResult RunExamples(Scenario scenario)
        {
            var runner = new ExampleRunner();
            Func<IEnumerable<StringStep>, IEnumerable<StepResult>> runSteps = steps => RunSteps(steps, BeforeStep, AfterStep);
            var exampleResults = runner.RunExamples(scenario, runSteps, BeforeScenario, AfterScenario);
            return exampleResults;
        }

        private IEnumerable<StepResult> RunBackground(Scenario background)
        {
            return RunSteps(background.Steps, ctx => { }, ctx => { })
                .Select(_ => new BackgroundStepResult(background.Title, _))
                .Cast<StepResult>()
                .ToList();
        }

        private IEnumerable<StepResult> RunSteps(IEnumerable<StringStep> stepsToRun,
            Action<StringStep> beforeStep,
            Action<StringStep> afterStep)
        {
            var failedStep = false;
            var stepResults = new List<StepResult>();
            foreach (var step in stepsToRun)
            {
                beforeStep(step);
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
                afterStep(step);
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