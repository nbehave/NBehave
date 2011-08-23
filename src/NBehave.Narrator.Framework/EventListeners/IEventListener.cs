namespace NBehave.Narrator.Framework.EventListeners
{
    public interface IEventListener
    {
        void RunStarted();
        void FeatureStarted(string feature);
        void FeatureNarrative(string message);
        void FeatureFinished(FeatureResult result);
        void ScenarioStarted(string scenarioTitle);
        void ScenarioFinished(ScenarioResult result);
        void RunFinished();
    }
}