using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ScenarioWithSteps
    {
        public static event EventHandler<EventArgs<ScenarioWithSteps>> ScenarioCreated;
        public static event EventHandler<EventArgs<ActionStepText>> StepAdded;

        private readonly List<StringStep> _steps = new List<StringStep>();
        private readonly List<Example> _examples = new List<Example>();
        private readonly IStringStepRunner _stringStepRunner;

        public ScenarioWithSteps(IStringStepRunner stringStepRunner)
        {
            Feature = new Feature();
            Title = string.Empty;
            _stringStepRunner = stringStepRunner;
        }

        public IEnumerable<StringStep> Steps
        {
            get { return _steps; }
        }

        public string Title { get; set; }
        public Feature Feature { get; set; }
        
        private string _source;
        public string Source 
        { 
        	get { return _source; }
        	set 
        	{
        		_source = value;
        		foreach(var step in Steps)
        			step.Source = _source;
        	}
        }
        		
        public IEnumerable<Example> Examples { get { return _examples; } }

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
                return RunExamples(Feature);

            return RunScenario(Feature, _steps);
        }

        private void OnScenarioCreated()
        {
            if (ScenarioCreated != null)
            {
                var e = new EventArgs<ScenarioWithSteps>(this);
                ScenarioCreated.Invoke(this, e);
            }
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
                _stringStepRunner.AfterScenario();
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

        private List<StringStep> CloneSteps()
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