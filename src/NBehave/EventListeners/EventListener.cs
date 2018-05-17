using System;

namespace NBehave.EventListeners
{
    public abstract class EventListener : MarshalByRefObject, IEventListener
    {
        public virtual void FeatureStarted(Feature feature)
        { }

        public virtual void FeatureFinished(FeatureResult result)
        {
        }

        public virtual void ScenarioStarted(string scenarioTitle)
        {
        }

        public virtual void ScenarioFinished(ScenarioResult result)
        {
        }

        public virtual void RunStarted()
        {
        }

        public virtual void RunFinished()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}