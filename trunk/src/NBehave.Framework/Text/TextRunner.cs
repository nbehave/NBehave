using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace NBehave.Narrator.Framework
{
    public class TextRunner : RunnerBase
    {
        private readonly List<List<ScenarioSteps>> _stories = new List<List<ScenarioSteps>>();
        private readonly ActionStepAlias _actionStepAlias = new ActionStepAlias();
        private readonly ActionStep _actionStep = new ActionStep();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly ActionStepRunner _actionStepRunner;
        private readonly TextToTokenStringsParser _textToTokenStringsParser;
        private IEventListener _listener;

        public ActionCatalog ActionCatalog { get; private set; }

        public TextRunner()
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _actionStepFileLoader = new ActionStepFileLoader(_actionStepAlias, _actionStep);
            _actionStepRunner = new ActionStepRunner(ActionCatalog);
            _textToTokenStringsParser = new TextToTokenStringsParser(_actionStepAlias, _actionStep);
        }

        protected override void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog);
            parser.FindActionSteps(assembly);
        }

        protected override void RunStories(StoryResults results, IEventListener listener)
        {
            _listener = listener;
            _listener.ThemeStarted(string.Empty);
            RunStories(results);
            _listener.ThemeFinished();
            ClearStoryList();
        }

        private void RunStories(StoryResults storyResults)
        {
            foreach (List<ScenarioSteps> scenarioSteps in _stories)
            {
                ScenarioStepRunner scenarioStepRunner = CreateScenarioStepRunner();

                scenarioStepRunner.RunScenarios(scenarioSteps, storyResults);
                storyResults.NumberOfStories++;
                _listener.StoryResults(storyResults);
            }
        }

        private ScenarioStepRunner CreateScenarioStepRunner()
        {
            var scenarioStepRunner = new ScenarioStepRunner(_textToTokenStringsParser, _actionStepRunner, _actionStep);
            scenarioStepRunner.EventListener = _listener;
            return scenarioStepRunner;
        }

        public void Load(IEnumerable<string> fileLocations)
        {
            _stories.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        public void Load(Stream stream)
        {
            List<ScenarioSteps> scenarios = _actionStepFileLoader.Load(stream);
            _stories.Add(scenarios);
        }
    }
}
