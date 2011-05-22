using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Processors
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using NBehave.Narrator.Framework.Contracts;
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioExecutor : IMessageProcessor 
    {
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;
        private IEnumerable<Feature> _features;
        private readonly Regex _hasParamsInStep = new Regex(@"\[\w+\]");
        private bool _actionStepsLoaded;

        public ScenarioExecutor(ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            _hub = hub;
            _stringStepRunner = stringStepRunner;

            _hub.Subscribe<FeaturesLoaded>(loaded =>
                {
                    _features = loaded.Content;
                    this.OnRunStarted();
                });

            _hub.Subscribe<ActionStepsLoaded>(stepsLoaded =>
                {
                    _actionStepsLoaded = true;
                    this.OnRunStarted();
                });
        }

        private void OnRunStarted()
        {
            if (_features == null || !_actionStepsLoaded) return;

            _hub.Publish(new ThemeStartedEvent(this, string.Empty));

            
            foreach (var feature in _features)
            {
                _hub.Publish(new FeatureCreatedEvent(this, feature.Title));
                _hub.Publish(new FeatureNarrativeEvent(this, feature.Narrative));

                Run(feature.Scenarios);
            }

            _hub.Publish(new ThemeFinishedEvent(this));

        }

        public void Run(IEnumerable<Scenario> scenarios)
        {
            foreach (var scenario in scenarios)
            {
                this._hub.Publish(new ScenarioCreatedEvent(this, scenario));
                if (scenario.Examples.Any())
                {
                    this.RunExamples(scenario);
                }
                else
                {
                    this.RunScenario(scenario);
                }
            }
        }

        private void RunScenario(Scenario scenario)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            _stringStepRunner.BeforeScenario();
            var stepsToRun = scenario.Feature.Background.Steps.Concat(scenario.Steps);
            foreach (var step in stepsToRun)
            {
                if (step is StringTableStep)
                {
                    RunStringTableStep((StringTableStep)step);
                }
                else if (step is StringStep)
                {
                    step.StepResult = _stringStepRunner.Run(step);                    
                }
                
                scenarioResult.AddActionStepResult(step.StepResult);
            }

            if (scenario.Steps.Any())
            {
                _stringStepRunner.AfterScenario();
            }

            this._hub.Publish(new ScenarioResultEvent(this, scenarioResult));
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
                foreach (var step in steps)
                {
                    if (step is StringTableStep)
                    {
                        RunStringTableStep((StringTableStep)step);
                    }
                    else if (step is StringStep)
                    {
                        step.StepResult = _stringStepRunner.Run(step);
                    }
                    scenarioResult.AddActionStepResult(step.StepResult);
                }

                if (scenario.Steps.Any())
                {
                    _stringStepRunner.AfterScenario();
                }

                exampleResults.AddResult(scenarioResult);
            }

            this._hub.Publish(new ScenarioResultEvent(this, exampleResults));
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

        private void InsertColumnValues(IEnumerable<StringStep> steps, Row example)
        {
            foreach (var step in steps)
            {
                foreach (var columnName in example.ColumnNames)
                {
                    var columnValue = example.ColumnValues[columnName].Trim();
                    var replace = new Regex(string.Format(@"(\${0})|(\[{0}\])", columnName), RegexOptions.IgnoreCase);
                    step.Step = replace.Replace(step.Step, columnValue);
                    
                    if(step is StringTableStep)
                    {
                        var tableSteps = ((StringTableStep) step).TableSteps;
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

        private IEnumerable<StringStep> CloneSteps(Scenario scenario)
        {
            var clones = new List<StringStep>();
            foreach (var step in scenario.Steps)
            {
                if (step is StringTableStep)
                {
                    var clone = new StringTableStep(step.Step, step.Source);
                    var tableSteps = ((StringTableStep) step).TableSteps;
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

        private ActionStepResult GetNewActionStepResult(StringTableStep stringStep)
        {
            var fullStep = CreateStepText(stringStep);
            return new ActionStepResult(fullStep, new Passed());
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
