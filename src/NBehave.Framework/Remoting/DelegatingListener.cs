using System;

namespace NBehave.Narrator.Framework.Remoting
{
    public class DelegatingListener : MarshalByRefObject, IEventListener
    {
        private readonly IEventListener _listener;

        public DelegatingListener(IEventListener listener)
        {
            _listener = listener;
        }

        public void FeatureCreated(string feature)
        {
            _listener.FeatureCreated(feature);
        }

        public void FeatureNarrative(string message)
        {
            _listener.FeatureNarrative(message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            _listener.ScenarioCreated(scenarioTitle);
        }

        public void RunStarted()
        {
            _listener.RunStarted();
        }

        public void RunFinished()
        {
            _listener.RunFinished();
        }

        public void ThemeStarted(string name)
        {
            _listener.ThemeStarted(name);
        }

        public void ThemeFinished()
        {
            _listener.ThemeFinished();
        }

        public void ScenarioResult(ScenarioResult result)
        {
            _listener.ScenarioResult(result);
        }
    }
}