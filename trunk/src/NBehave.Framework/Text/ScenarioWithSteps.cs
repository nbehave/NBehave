using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ScenarioWithSteps
    {
        private readonly List<StringStep> _steps = new List<StringStep>();
        private readonly List<Example> _examples = new List<Example>();
        private readonly StringStepRunner _stringStepRunner;
        private readonly IEventListener _listener;

        public ScenarioWithSteps(StringStepRunner stringStepRunner, IEventListener listener)
        {
            Feature = new Feature();
            Title = string.Empty;
            _stringStepRunner = stringStepRunner;
            _listener = listener;
        }

        public IEnumerable<StringStep> Steps
        {
            get { return _steps; }
        }

        public string Title { get; set; }
        public Feature Feature { get; set; }
        public string FileName { get; set; }

        public IEnumerable<Example> Examples { get { return _examples; } }

        public void AddStep(string step)
        {
            var stringStringStep = new StringStep(step, FileName, _stringStepRunner, _listener);
            AddStep(stringStringStep);
        }

        public void AddStep(StringStep step)
        {
            _steps.Add(step);
        }

        public void AddExamples(List<Example> examples)
        {
            _examples.AddRange(examples);
        }

        public virtual IEnumerable<ScenarioResult> Run()
        {
            Story story = Feature.AsStory();
            _listener.StoryMessageAdded(Feature.Narrative);
            if (Examples.Any())
                return RunExamples(story);

            return RunScenario(story, _steps);
        }

        private IEnumerable<ScenarioResult> RunScenario(Story story, IEnumerable<StringStep> stepsToRun)
        {
            var scenarioResult = new ScenarioResult(story, Title);
            _stringStepRunner.BeforeScenario();
            _listener.ScenarioCreated(Title);
            foreach (var step in stepsToRun)
            {
                ActionStepResult stepResult = step.Run();
                scenarioResult.AddActionStepResult(stepResult);
            }
            if (stepsToRun.Any())
                _stringStepRunner.AfterScenario();
            return new List<ScenarioResult> { scenarioResult };
        }

        private IEnumerable<ScenarioResult> RunExamples(Story story)
        {
            var results = new List<ScenarioResult>();
            foreach (var example in Examples)
            {
                var steps = CloneSteps();
                InsertColumnValues(steps, example);
                var tmp = RunScenario(story, steps);
                results.AddRange(tmp);
            }
            return results;
        }

        private void InsertColumnValues(IEnumerable<StringStep> steps, Row example)
        {
            foreach (var step in steps)
            {
                foreach (var columnName in example.ColumnNames)
                {
                    var columnValue = example.ColumnValues[columnName];
                    Regex replace = new Regex(string.Format(@"(\${0})|(\[{0}\])", columnName), RegexOptions.IgnoreCase);
                    step.Step = replace.Replace(step.Step, columnValue);
                }
            }
        }

        private List<StringStep> CloneSteps()
        {
            var clones = new List<StringStep>();
            foreach (var step in Steps)
            {
                var s = new StringStep(step.Step, step.FromFile, _stringStepRunner, _listener);
                clones.Add(s);
            }
            return clones;
        }
    }
}