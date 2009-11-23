namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class StoryResultsEventReceived : EventReceived
    {
        public StoryResults StoryResults {get; private set; }
		
        public StoryResultsEventReceived(StoryResults results)
            : base("", EventType.StoryResult)
        {
            StoryResults = results;
        }
    }
}