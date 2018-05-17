using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Internal
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
            var featureResult = new FeatureResult(feature.Title);
            foreach (var scenario in feature.Scenarios)
            {
                var backgroundResults = RunBackground(scenario.Feature.Background);
                var scenarioResult = BeforeScenario(scenario);
                scenarioResult.AddActionStepResults(backgroundResults);
                if (!scenarioResult.HasFailed)
                {
                    scenarioResult = scenario.Examples.Any() ? RunExamples(scenario) : RunScenario(scenario);
                }
                AfterScenario(scenarioResult);
                featureResult.AddResult(scenarioResult);
            }
            return featureResult;
        }

        private ScenarioResult BeforeScenario(Scenario scenario)
        {
            var result = new ScenarioResult(scenario.Feature, scenario.Title);

            try
            {
                context.ScenarioStartedEvent(scenario);
            }
            catch (Exception e)
            {
                result.Fail(e);
            }
            return result;
        }

        private void AfterScenario(ScenarioResult scenarioResult)
        {
            try
            {
                context.ScenarioFinishedEvent(scenarioResult);
            }
            catch (Exception e)
            {
                if (!scenarioResult.HasFailed)
                    scenarioResult.Fail(e);
            }
        }

        private ScenarioResult RunScenario(Scenario scenario)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            var stepResults = RunSteps(scenario.Steps, BeforeStep, AfterStep);

            scenarioResult.AddActionStepResults(stepResults);
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
            var exampleResults = runner.RunExamples(scenario, runSteps, () => { }, (a, b) => { });
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
            Action<StringStep> beforeStep, Action<StringStep> afterStep)
        {
            var failedStep = false;
            var stepResults = new List<StepResult>();
            foreach (var step in stepsToRun)
            {
                InvokeAction(beforeStep, step);
                if (failedStep)
                    step.PendBecauseOfPreviousFailedStep();
                else
                    stringStepRunner.Run(step);

                InvokeAction(afterStep, step);
                stepResults.Add(step.StepResult);
                if (step.StepResult.Result is Failed)
                    failedStep = true;
            }
            return stepResults;
        }

        private void InvokeAction(Action<StringStep> action, StringStep step)
        {
            try
            {
                action(step);
            }
            catch (Exception e)
            {
                if (!(step.StepResult.Result is Failed))
                    step.Fail(e);
            }
        }
    }
}