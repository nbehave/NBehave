namespace NBehave.EventListeners
{
    public interface IEventListener
    {
        void RunStarted();
        void FeatureStarted(Feature feature);
        void FeatureFinished(FeatureResult result);
        void ScenarioStarted(string scenarioTitle);
        void ScenarioFinished(ScenarioResult result);
        void RunFinished();
    }
}