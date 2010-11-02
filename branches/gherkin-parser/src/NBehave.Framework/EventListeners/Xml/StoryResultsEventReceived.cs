namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class ScenarioResultEventReceived : EventReceived
    {
        public ScenarioResult ScenarioResult {get; private set; }

        public ScenarioResultEventReceived(ScenarioResult results)
            : base("", EventType.ScenarioResult)
        {
            ScenarioResult = results;
        }
    }
}