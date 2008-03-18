namespace NBehave.Narrator.Framework.EventListeners
{
    public class NullEventListener : IEventListener
    {
        #region IEventListener Members

        public void StoryCreated(string story)
        {
        }

        public void StoryMessageAdded(string message)
        {
        }

        public void RunStarted()
        {
        }

        public void RunFinished()
        {
        }

        public void ThemeStarted(string name)
        {
        }

        public void ThemeFinished()
        {
        }

        public void StoryResults(StoryResults results)
        {
        }

        #endregion
    }
}
