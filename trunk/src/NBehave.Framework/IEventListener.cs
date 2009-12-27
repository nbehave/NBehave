namespace NBehave.Narrator.Framework
{
    public interface IEventListener
    {
        void StoryCreated(string story);
        void StoryMessageAdded(string message);
        void ScenarioCreated(string scenarioTitle);
        void ScenarioMessageAdded(string message);
        void RunStarted();
        void RunFinished();
        void ThemeStarted(string name);
        void ThemeFinished();

        //void StoryResults(FeatureResults results);
        void ScenarioResult(ScenarioResult result);
    }
}