// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioWithSteps.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioWithSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioWithSteps
    {
        private readonly List<StringStep> _steps;
        private readonly List<Example> _examples;
        private readonly IStringStepRunner _stringStepRunner;

        private readonly ITinyMessengerHub _hub;

        private string _source;

        public ScenarioWithSteps(IStringStepRunner stringStepRunner, ITinyMessengerHub hub)
        {
            Feature = new Feature();
            Title = string.Empty;
            _stringStepRunner = stringStepRunner;
            _hub = hub;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public static event EventHandler<EventArgs<ActionStepText>> StepAdded;

        public string Title { get; set; }

        public Feature Feature { get; set; }

        public IEnumerable<StringStep> Steps
        {
            get { return _steps; }
        }

        public string Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
                foreach (var step in Steps)
                {
                    step.Source = _source;
                }
            }
        }

        public IEnumerable<Example> Examples
        {
            get
            {
                return _examples;
            }
        }

        public void AddStep(string step)
        {
            var stringStringStep = new StringStep(step, Source, _stringStepRunner);
            AddStep(stringStringStep);
        }

        public void AddStep(StringStep step)
        {
            _steps.Add(step);
            OnStepAdded(step);
        }

        public void AddExamples(List<Example> examples)
        {
            _examples.AddRange(examples);
        }

        public virtual ScenarioResult Run()
        {
            OnScenarioCreated();
            if (Examples.Any())
            {
                return RunExamples(Feature);
            }

            return RunScenario(Feature, _steps);
        }

        public void RemoveLastStep()
        {
            _steps.Remove(_steps.Last());
        }

        private void OnScenarioCreated()
        {
            _hub.Publish(new ScenarioCreated(this, Title));
        }

        private void OnStepAdded(ActionStepText step)
        {
            if (StepAdded != null)
            {
                var e = new EventArgs<ActionStepText>(step);
                StepAdded.Invoke(this, e);
            }
        }

        private ScenarioResult RunScenario(Feature feature, IEnumerable<StringStep> stepsToRun)
        {
            var scenarioResult = new ScenarioResult(feature, Title);
            _stringStepRunner.BeforeScenario();
            foreach (var step in stepsToRun)
            {
                step.Run();
                scenarioResult.AddActionStepResult(step.StepResult);
            }

            if (stepsToRun.Any())
            {
                _stringStepRunner.AfterScenario();
            }

            return scenarioResult;
        }

        private ScenarioResult RunExamples(Feature feature)
        {
            var exampleResults = new ScenarioExampleResult(feature, Title, Steps, Examples);

            foreach (var example in Examples)
            {
                var steps = CloneSteps();
                InsertColumnValues(steps, example);
                var exampleResult = RunScenario(feature, steps);
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

        private IEnumerable<StringStep> CloneSteps()
        {
            var clones = new List<StringStep>();
            foreach (var step in Steps)
            {
                var s = new StringStep(step.Step, step.Source, _stringStepRunner);
                clones.Add(s);
            }

            return clones;
        }
    }
}