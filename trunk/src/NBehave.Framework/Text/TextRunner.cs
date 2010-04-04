using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class TextRunner : RunnerBase
    {
        private readonly List<Feature> _features = new List<Feature>();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly IStringStepRunner _stringStepRunner;
        
        public ActionCatalog ActionCatalog { get; private set; }

        public TextRunner(IEventListener eventListener) 
            : base(eventListener)
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _stringStepRunner = new StringStepRunner(ActionCatalog);
            _actionStepFileLoader = new ActionStepFileLoader(_stringStepRunner);
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
            foreach (var feature in _features)
            {
                ScenarioStepRunner scenarioStepRunner = CreateScenarioStepRunner();

                IEnumerable<ScenarioResult> scenarioResults = scenarioStepRunner.Run(feature.Scenarios);
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
            _features.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        public void Load(Stream stream)
        {
            var features = _actionStepFileLoader.Load(stream);
            _features.AddRange(features);
        }
    }
}
