using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioExecutor : IMessageProcessor 
    {
        private readonly ITinyMessengerHub _hub;
        private IEnumerable<Feature> _features;

        public ScenarioExecutor(ITinyMessengerHub hub)
        {
            _hub = hub;

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
                var scenarios = feature.Scenarios;

                var scenarioResults = Run(scenarios);

                AddScenarioResultsToStoryResults(scenarioResults, featureResults);
                featureResults.NumberOfStories++;
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

                var scenarioResults = scenario.Run();

                this._hub.Publish(new ScenarioResultMessage(this, scenarioResults));

                allResults.Add(scenarioResults);
            }

            return allResults;
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
