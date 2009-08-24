using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace NBehave.Narrator.Framework
{
    public class TextRunner : RunnerBase
    {
        private readonly List<string> _scenarios = new List<string>();
        private readonly ActionStepAlias _actionStepAlias = new ActionStepAlias();
        private readonly ActionStep _actionStep = new ActionStep();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly ActionStepRunner _actionStepRunner;
        private readonly ScenarioStepRunner _scenarioStepRunner;


        public ActionCatalog ActionCatalog { get; private set; }

        public TextRunner()
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _actionStepFileLoader = new ActionStepFileLoader(_actionStepAlias, _actionStep);
            _actionStepRunner = new ActionStepRunner(ActionCatalog);
            var textToTokenStringsParser = new TextToTokenStringsParser(_actionStepAlias, _actionStep);
            _scenarioStepRunner = new ScenarioStepRunner(textToTokenStringsParser, _actionStepRunner, _actionStep);
        }

        protected override void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog, _actionStepAlias);
            parser.FindActionSteps(assembly);
        }

        protected override void RunStories(StoryResults results, IEventListener listener)
        {
            listener.ThemeStarted(string.Empty);
            _scenarioStepRunner.EventListener = listener;
            RunScenarios(results);
            listener.StoryResults(results);
            listener.ThemeFinished();
            ClearStoryList();
        }

        private void RunScenarios(StoryResults storyResults)
        {
            int scenarioCounter = 0;
            foreach (string scenarioText in _scenarios)
            {
                scenarioCounter++;
                _scenarioStepRunner.RunScenario(scenarioText, storyResults, scenarioCounter);
            }
        }

        public void Load(IEnumerable<string> scenarioLocations)
        {
            var scenarios = _actionStepFileLoader.Load(scenarioLocations);
            _scenarios.AddRange(scenarios);
        }

        public void Load(Stream stream)
        {
            var scenarios = _actionStepFileLoader.Load(stream);
            _scenarios.AddRange(scenarios);
        }
    }
}
