using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class TextRunner : RunnerBase
    {
        private readonly List<List<ScenarioWithSteps>> _stories = new List<List<ScenarioWithSteps>>();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly StringStepRunner _stringStepRunner;
        
        public ActionCatalog ActionCatalog { get; private set; }

        public TextRunner(IEventListener eventListener) 
            : base(eventListener)
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _stringStepRunner = new StringStepRunner(ActionCatalog);
            _actionStepFileLoader = new ActionStepFileLoader(_stringStepRunner, EventListener);
        }

        protected override void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog);
            parser.FindActionSteps(assembly);
        }

        protected override void RunStories(StoryResults results)
        {
            EventListener.ThemeStarted(string.Empty);
            RunEachStory(results);
            EventListener.ThemeFinished();
            ClearStoryList();
        }

        private void RunEachStory(StoryResults storyResults)
        {
            foreach (List<ScenarioWithSteps> scenarioSteps in _stories)
            {
                ScenarioStepRunner scenarioStepRunner = CreateScenarioStepRunner();

                IEnumerable<ScenarioResult> scenarioResults = scenarioStepRunner.RunScenarios(scenarioSteps);
                AddScenarioResultsToStoryResults(scenarioResults, storyResults);
                storyResults.NumberOfStories++;
                EventListener.StoryResults(storyResults);
            }
        }

        private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, StoryResults storyResults)
        {
            foreach (var result in scenarioResults)
                storyResults.AddResult(result);
        }

        private ScenarioStepRunner CreateScenarioStepRunner()
        {
            var scenarioStepRunner = new ScenarioStepRunner();
            scenarioStepRunner.EventListener = EventListener;
            return scenarioStepRunner;
        }

        public void Load(IEnumerable<string> fileLocations)
        {
            _stories.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        public void Load(Stream stream)
        {
            List<ScenarioWithSteps> scenarios = _actionStepFileLoader.Load(stream);
            _stories.Add(scenarios);
        }
    }
}
