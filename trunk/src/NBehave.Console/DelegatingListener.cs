using System;
using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    public class DelegatingListener : MarshalByRefObject, IEventListener
    {
        private IEventListener _localListener;

        public DelegatingListener(IEventListener localListener)
        {
            _localListener = localListener;
        }

        public void StoryCreated(string story)
        {
            _localListener.StoryCreated(story);
        }

        public void StoryMessageAdded(string message)
        {
            _localListener.StoryMessageAdded(message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            _localListener.ScenarioCreated(scenarioTitle);
        }

        public void ScenarioMessageAdded(string message)
        {
            _localListener.ScenarioMessageAdded(message);
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

        public void StoryResults(StoryResults results)
        {
            _localListener.StoryResults(results);
        }
    }
}
