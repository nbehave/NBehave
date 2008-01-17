namespace NBehave.Narrator.Framework.EventListeners
{
    public class NullEventListener : IEventListener
    {
        #region IEventListener Members

        public void StoryCreated()
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

        #endregion
    }
}
