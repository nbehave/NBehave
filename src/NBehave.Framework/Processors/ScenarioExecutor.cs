using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioExecutor : IMessageProcessor 
    {
        private readonly NBehaveConfiguration _configuration;

        private readonly ITinyMessengerHub _hub;
        private IEnumerable<Feature> _features;

        public ScenarioExecutor(NBehaveConfiguration configuration, ITinyMessengerHub hub)
        {
            this._configuration = configuration;
            _hub = hub;

            _hub.Subscribe<FeaturesLoaded>(loaded => _features = loaded.Content);
            _hub.Subscribe<RunStarted>(this.OnRunStarted);
        }

        public void Start()
        {
        }

        private void OnRunStarted(RunStarted runStarted)
        {
            _configuration.EventListener.ThemeStarted(string.Empty);
            var featureResults = new FeatureResults(this);
            foreach (var feature in _features.Where(feature => true))
            {
                var scenarios = feature.Scenarios.Where(steps => true);
                var scenarioStepRunner = new ScenarioStepRunner();

                var scenarioResults = scenarioStepRunner.Run(scenarios);
                AddScenarioResultsToStoryResults(scenarioResults, featureResults);
                featureResults.NumberOfStories++;
            }

            _configuration.EventListener.ThemeFinished();

            _hub.Publish(featureResults);
        }

        private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, FeatureResults featureResults)
        {
            foreach (var result in scenarioResults)
            {
                featureResults.AddResult(result);
            }
        }
    }
}
