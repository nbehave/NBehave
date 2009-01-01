namespace NBehave.Narrator.Framework.EventListeners
{
    public class NullEventListener : IEventListener
    {
         void IEventListener.StoryCreated(string story)
        {
        }

        void IEventListener.StoryMessageAdded(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {            
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.RunFinished()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        void IEventListener.StoryResults(StoryResults results)
        {
        }
    }
}
