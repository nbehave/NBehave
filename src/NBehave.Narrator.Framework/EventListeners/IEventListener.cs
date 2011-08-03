namespace NBehave.Narrator.Framework.EventListeners
{
    public interface IEventListener
    {
        void RunStarted();
        void ThemeStarted(string name);
        void FeatureStarted(string feature);
        void FeatureNarrative(string message);
        void ScenarioStarted(string scenarioTitle);
        void ScenarioResult(ScenarioResult result);
        void ThemeFinished();
        void RunFinished();
    }
}