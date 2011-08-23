using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class FeatureResultCollector : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;

        private FeatureResultEvent _featureResultEvent;

        public FeatureResultCollector(ITinyMessengerHub hub)
        {
            _featureResultEvent = new FeatureResultEvent(this, new FeatureResult());
            _hub = hub;
            _hub.Subscribe<FeatureStartedEvent>(OnFeatureStarted);
            _hub.Subscribe<ScenarioResultEvent>(OnScenarioResultRecieved);
            _hub.Subscribe<FeatureFinishedEvent>(finished => OnFeatureFinished());
        }

        private void OnFeatureStarted(FeatureStartedEvent obj)
        {
            _featureResultEvent = new FeatureResultEvent(this, new FeatureResult());
        }

        private void OnScenarioResultRecieved(ScenarioResultEvent message)
        {
            _featureResultEvent.Content.AddResult(message.Content);
        }

        private void OnFeatureFinished()
        {
            _hub.Publish(_featureResultEvent);
        }
    }
}