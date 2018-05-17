using System;

namespace NBehave.EventListeners
{
    public class MultiOutputEventListener : EventListener
    {
        private readonly IEventListener[] listeners;

        public MultiOutputEventListener(params IEventListener[] listeners)
        {
            this.listeners = listeners;
        }

        public IEventListener[] Listeners
        {
            get { return listeners; }
        }

        public override void FeatureStarted(Feature feature)
        {
            Invoke(l => l.FeatureStarted(feature));
        }

        public override void ScenarioStarted(string scenarioTitle)
        {
            Invoke(l => l.ScenarioStarted(scenarioTitle));
        }

        public override void RunStarted()
        {
            Invoke(l => l.RunStarted());
        }

        public override void RunFinished()
        {
            Invoke(l => l.RunFinished());
        }

        public override void FeatureFinished(FeatureResult result)
        {
            Invoke(l => l.FeatureFinished(result));
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            Invoke(l => l.ScenarioFinished(result));
        }

        private void Invoke(Action<IEventListener> f)
        {
            foreach (var listener in listeners)
            {
                f(listener);
            }
        }
    }
}
