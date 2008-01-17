using System.Reflection;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class MultiOutputEventListener : IEventListener
    {
        private readonly IEventListener[] listeners;

        public MultiOutputEventListener(params IEventListener[] listeners)
        {
            this.listeners = listeners;
        }

        #region IEventListener Members

        public void StoryCreated()
        {
            Invoke(MethodInfo.GetCurrentMethod().Name);
        }

        public void StoryMessageAdded(string message)
        {
            Invoke(MethodInfo.GetCurrentMethod().Name, message);
        }

        public void RunStarted()
        {
            Invoke(MethodInfo.GetCurrentMethod().Name);
        }

        public void RunFinished()
        {
            Invoke(MethodInfo.GetCurrentMethod().Name);
        }

        public void ThemeStarted(string name)
        {
            Invoke(MethodInfo.GetCurrentMethod().Name, name);
        }

        public void ThemeFinished()
        {
            Invoke(MethodInfo.GetCurrentMethod().Name);
        }

        #endregion

        private void Invoke(string methodName, params object[] args)
        {
            foreach (IEventListener listener in listeners)
            {
                typeof (IEventListener).GetMethod(methodName).Invoke(listener, args);
            }
        }
    }
}
