using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;

        public ScenarioRunner(ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            _hub = hub;
            _stringStepRunner = stringStepRunner;
        }

        public void Run(Feature feature)
        {
            foreach (var scenario in feature.Scenarios)
            {
                _hub.Publish(new ScenarioStartedEvent(this, scenario));
                if (scenario.Examples.Any())
                    RunExamples(scenario);
                else
                    RunScenario(scenario);
                _hub.Publish(new ScenarioFinishedEvent(this, scenario));
            }
        }

        private void RunScenario(Scenario scenario)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            _stringStepRunner.BeforeScenario();
            var backgroundResults = RunBackground(scenario.Feature.Background);
            scenarioResult.AddActionStepResults(backgroundResults);
            var stepResults = RunSteps(scenario.Steps);
            scenarioResult.AddActionStepResults(stepResults);
            ExecuteAfterScenario(scenario, scenarioResult);
            _hub.Publish(new ScenarioResultEvent(this, scenarioResult));
        }

        private void RunExamples(Scenario scenario)
        {
            var exampleResults = new ScenarioExampleResult(scenario.Feature, scenario.Title, scenario.Steps, scenario.Examples);

            foreach (var example in scenario.Examples)
            {
                var steps = CloneSteps(scenario);
                InsertColumnValues(steps, example);

                var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
                _stringStepRunner.BeforeScenario();
                var stepResults = RunSteps(steps);
                scenarioResult.AddActionStepResults(stepResults);
                ExecuteAfterScenario(scenario, scenarioResult);
                exampleResults.AddResult(scenarioResult);
            }
            _hub.Publish(new ScenarioResultEvent(this, exampleResults));
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
                if (failedStep)
                {
                    step.StepResult = new StepResult(step, new PendingBecauseOfPreviousFailedStep("Previous step has failed"));
                    stepResults.Add(step.StepResult);
                    continue;
                }

                if (step is StringTableStep)
                    RunStringTableStep((StringTableStep)step);
                else 
                    step.StepResult = _stringStepRunner.Run(step);

                if (step.StepResult.Result is Failed)
                {
                    failedStep = true;
                }
                stepResults.Add(step.StepResult);
            }
            return stepResults;
        }
       
        private void ExecuteAfterScenario(Scenario scenario, ScenarioResult scenarioResult)
        {
            if (scenario.Steps.Any())
            {
                try
                {
                    _stringStepRunner.AfterScenario();
                }
                catch (Exception e)
                {
                    if (!scenarioResult.HasFailedSteps())
                        scenarioResult.Fail(e);
                }
            }
        }

        private void RunStringTableStep(StringTableStep stringStep)
        {
            var r = new StringTableStepRunner(_stringStepRunner);
            r.RunStringTableStep(stringStep);
        }

        private void InsertColumnValues(IEnumerable<StringStep> steps, Row example)
        {
            foreach (var step in steps)
            {
                foreach (var columnName in example.ColumnNames)
                {
                    var columnValue = example.ColumnValues[columnName.Name].TrimWhiteSpaceChars();
                    var replace = new Regex(string.Format(@"(\${0})|(\[{0}\])", columnName), RegexOptions.IgnoreCase);
                    step.Step = replace.Replace(step.Step, columnValue);

                    if (step is StringTableStep)
                    {
                        var tableSteps = ((StringTableStep)step).TableSteps;
                        foreach (var row in tableSteps)
                        {
                            var newValues = row.ColumnValues.ToDictionary(pair => pair.Key, pair => replace.Replace(pair.Value, columnValue));
                            row.ColumnValues.Clear();
                            foreach (var pair in newValues)
                            {
                                row.ColumnValues.Add(pair.Key, pair.Value);
                            }
                        }
                    }
                }
            }
        }

        private ICollection<StringStep> CloneSteps(Scenario scenario)
        {
            var clones = new List<StringStep>();
            foreach (var step in scenario.Steps)
            {
                if (step is StringTableStep)
                {
                    var clone = new StringTableStep(step.Step, step.Source);
                    var tableSteps = ((StringTableStep)step).TableSteps;
                    foreach (var tableStep in tableSteps)
                    {
                        var clonedValues = tableStep.ColumnValues.ToDictionary(pair => pair.Key, pair => pair.Value);
                        var clonedNames = new ExampleColumns(tableStep.ColumnNames);
                        var clonedRow = new Row(clonedNames, clonedValues);
                        clone.AddTableStep(clonedRow);
                    }
                    clones.Add(clone);
                }
                else
                {
                    clones.Add(new StringStep(step.Step, step.Source));
                }
            }

            return clones;
        }
    }
}