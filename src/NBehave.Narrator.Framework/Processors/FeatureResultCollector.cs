using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class FeatureResultCollector : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;

        private readonly FeatureResults _featureResults;

        public FeatureResultCollector(ITinyMessengerHub hub)
        {
            _hub = hub;
            _featureResults = new FeatureResults(this);

            _hub.Subscribe<ScenarioResultEvent>(OnScenarioResultRecieved);
            _hub.Subscribe<ThemeFinishedEvent>(finished => OnThemeFinished());
        }

        private void OnScenarioResultRecieved(ScenarioResultEvent message)
        {
            _featureResults.AddResult(message.Content);
        }

        private void OnThemeFinished()
        {
            _hub.Publish(_featureResults);
        }
    }
}