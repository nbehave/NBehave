using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Processors
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioExecutor : IMessageProcessor 
    {
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;
        private IEnumerable<Feature> _features;
        private readonly Regex _hasParamsInStep = new Regex(@"\[\w+\]");

        public ScenarioExecutor(ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            _hub = hub;
            _stringStepRunner = stringStepRunner;

            _hub.Subscribe<FeaturesLoaded>(loaded => _features = loaded.Content);
            _hub.Subscribe<RunStarted>(this.OnRunStarted);
        }

        public void Start()
        {
        }

        private void OnRunStarted(RunStarted runStarted)
        {
            _hub.Publish(new ThemeStarted(this, string.Empty));

            var featureResults = new FeatureResults(this);
            
            foreach (var feature in _features)
            {
                var scenarioResults = Run(feature.Scenarios);

                foreach (var result in scenarioResults)
                {
                    featureResults.AddResult(result);
                }
            }

            _hub.Publish(new ThemeFinished(this));

            _hub.Publish(featureResults);
        }

        public IEnumerable<ScenarioResult> Run(IEnumerable<ScenarioWithSteps> scenarios)
        {
            var allResults = new List<ScenarioResult>();

            Feature lastFeature = null;

            foreach (var scenario in scenarios)
            {
                if (scenario.Feature != lastFeature)
                {
                    lastFeature = scenario.Feature;

                    _hub.Publish(new FeatureCreated(this, lastFeature.Title));
                    _hub.Publish(new FeatureNarrative(this, lastFeature.Narrative));
                }

                var scenarioResults = this.Run(scenario);

                this._hub.Publish(new ScenarioResultMessage(this, scenarioResults));

                allResults.Add(scenarioResults);
            }

            return allResults;
        }

        public ScenarioResult Run(ScenarioWithSteps scenario)
        {
            this._hub.Publish(new ScenarioCreated(this, scenario.Title));
            if (scenario.Examples.Any())
            {
                return RunExamples(scenario);
            }

            return RunScenario(scenario, scenario.Steps);
        }

        private ScenarioResult RunScenario(ScenarioWithSteps scenario, IEnumerable<StringStep> stepsToRun)
        {
            var scenarioResult = new ScenarioResult(scenario.Feature, scenario.Title);
            _stringStepRunner.BeforeScenario();
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

            return scenarioResult;
        }

        private ScenarioResult RunExamples(ScenarioWithSteps scenario)
        {
            var exampleResults = new ScenarioExampleResult(scenario.Feature, scenario.Title, scenario.Steps, scenario.Examples);

            foreach (var example in scenario.Examples)
            {
                var steps = CloneSteps(scenario);
                InsertColumnValues(steps, example);
                var exampleResult = RunScenario(scenario, steps);
                exampleResults.AddResult(exampleResult);
            }

            return exampleResults;
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
                }
            }
        }

        private IEnumerable<StringStep> CloneSteps(ScenarioWithSteps scenario)
        {
            var clones = new List<StringStep>();
            foreach (var step in scenario.Steps)
            {
                var s = new StringStep(step.Step, step.Source);
                clones.Add(s);
            }

            return clones;
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
