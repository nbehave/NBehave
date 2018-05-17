namespace NBehave.EventListeners.Xml
{
    public class ScenarioResultEventReceived : EventReceived
    {
        public ScenarioResultEventReceived(ScenarioResult results) 
            : base(string.Empty, EventType.ScenarioResult)
        {
            ScenarioResult = results;
        }

        public ScenarioResult ScenarioResult { get; private set; }
    }
}