using System.Reflection;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class MultiOutputEventListener : IEventListener
    {
        private readonly IEventListener[] _listeners;

        public MultiOutputEventListener(params IEventListener[] listeners)
        {
            _listeners = listeners;
        }

        public IEventListener[] Listeners
        {
            get { return _listeners; }
        }

        #region IEventListener Members

        public void StoryCreated(string story)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, story);
        }

        public void StoryMessageAdded(string message)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, scenarioTitle);            
        }

        public void ScenarioMessageAdded(string message)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, message);
        }

        public void RunStarted()
        {
            Invoke(MethodBase.GetCurrentMethod().Name);
        }

        public void RunFinished()
        {
            Invoke(MethodBase.GetCurrentMethod().Name);
        }

        public void ThemeStarted(string name)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, name);
        }

        public void ThemeFinished()
        {
            Invoke(MethodBase.GetCurrentMethod().Name);
        }

        public void StoryResults(StoryResults results)
        {
            Invoke(MethodBase.GetCurrentMethod().Name, results);
        }

        #endregion

        private void Invoke(string methodName, params object[] args)
        {
            foreach (IEventListener listener in Listeners)
            {
                typeof (IEventListener).GetMethod(methodName).Invoke(listener, args);
            }
        }
    }
}
