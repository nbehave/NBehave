using System;
using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    public class DelegatingListener : MarshalByRefObject, IEventListener
    {
        private readonly IEventListener _localListener;

        public DelegatingListener(IEventListener localListener)
        {
            _localListener = localListener;
        }

        public void FeatureCreated(string feature)
        {
            _localListener.FeatureCreated(feature);
        }

        public void FeatureNarrative(string message)
        {
            _localListener.FeatureNarrative(message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            _localListener.ScenarioCreated(scenarioTitle);
        }

        public void RunStarted()
        {
            _localListener.RunStarted();
        }

        public void RunFinished()
        {
            _localListener.RunFinished();
        }

        public void ThemeStarted(string name)
        {
            _localListener.ThemeStarted(name);
        }

        public void ThemeFinished()
        {
            _localListener.ThemeFinished();
        }

        public void ScenarioResult(ScenarioResult results)
        {
            _localListener.ScenarioResult(results);
        }
    }
}
