namespace NBehave.Narrator.Framework.EventListeners
{
    public interface IEventListener
    {
        void FeatureCreated(string feature);
        void FeatureNarrative(string message);
        void ScenarioCreated(string scenarioTitle);
        void RunStarted();
        void RunFinished();
        void ThemeStarted(string name);
        void ThemeFinished();
        void ScenarioResult(ScenarioResult result);
    }
}