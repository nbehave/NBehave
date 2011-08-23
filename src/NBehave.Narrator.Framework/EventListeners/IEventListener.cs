namespace NBehave.Narrator.Framework.EventListeners
{
    public interface IEventListener
    {
        void RunStarted();
        void FeatureStarted(string feature);
        void FeatureNarrative(string message);
        void ScenarioStarted(string scenarioTitle);
        void ScenarioResult(ScenarioResult result);
        void FeatureFinished();
        void RunFinished();
    }
}