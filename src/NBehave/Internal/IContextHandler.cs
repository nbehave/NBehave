namespace NBehave.Narrator.Framework.Internal
{
    public interface IContextHandler
    {
        void OnFeatureStartedEvent(Feature feature);
        void OnFeatureFinishedEvent();
        void OnScenarioStartedEvent(Scenario scenario);
        void OnScenarioFinishedEvent();
        void OnStepStartedEvent(StringStep step);
        void OnStepFinishedEvent();
    }
}