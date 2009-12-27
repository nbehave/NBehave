using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class TextRunner : RunnerBase
    {
        private readonly List<List<ScenarioWithSteps>> _scenarios = new List<List<ScenarioWithSteps>>();
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

        protected override void RunFeatures(FeatureResults results)
        {
            EventListener.ThemeStarted(string.Empty);
            RunEachFeature(results);
            EventListener.ThemeFinished();
        }

        private void RunEachFeature(FeatureResults featureResults)
        {
            foreach (List<ScenarioWithSteps> scenarioSteps in _scenarios)
            {
                ScenarioStepRunner scenarioStepRunner = CreateScenarioStepRunner();

                IEnumerable<ScenarioResult> scenarioResults = scenarioStepRunner.Run(scenarioSteps);
                AddScenarioResultsToStoryResults(scenarioResults, featureResults);
                featureResults.NumberOfStories++;
                //EventListener.StoryResults(featureResults);
            }
        }

        private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, FeatureResults featureResults)
        {
            foreach (var result in scenarioResults)
                featureResults.AddResult(result);
        }

        private ScenarioStepRunner CreateScenarioStepRunner()
        {
            var scenarioStepRunner = new ScenarioStepRunner();
            return scenarioStepRunner;
        }

        public void Load(IEnumerable<string> fileLocations)
        {
            _scenarios.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        public void Load(Stream stream)
        {
            List<ScenarioWithSteps> scenarios = _actionStepFileLoader.Load(stream);
            _scenarios.Add(scenarios);
        }
    }
}
