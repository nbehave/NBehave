namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureResultCollector : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;

        private FeatureResults _featureResults;

        public FeatureResultCollector(ITinyMessengerHub hub)
        {
            _hub = hub;
            this._featureResults = new FeatureResults(this);

            _hub.Subscribe<ScenarioResultEvent>(this.OnScenarioResultRecieved);
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
