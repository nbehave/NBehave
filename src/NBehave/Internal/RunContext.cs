using System;
using NBehave.Domain;
using NBehave.Hooks;

namespace NBehave.Internal
{
    public class RunContext : IRunContext, IRunContextEvents
    {
        private readonly IContextHandler contextHandler;
        private readonly HooksHandler hooksHandler;

        public RunContext(IContextHandler contextHandler, HooksHandler hooksHandler)
        {
            this.contextHandler = contextHandler;
            this.hooksHandler = hooksHandler;
            this.hooksHandler.SubscribeToHubEvents(this);
        }

        public void RunStarted()
        {
            if (OnRunStarted != null)
                OnRunStarted(this, new EventArgs());
        }

        public void RunFinished(FeatureResults results)
        {
            if (OnRunFinished != null)
                OnRunFinished(this, new EventArgs<FeatureResults>(results));
        }

        public void FeatureStarted(Feature feature)
        {
            contextHandler.OnFeatureStartedEvent(feature);
            if (OnFeatureStarted != null)
                OnFeatureStarted(this, new EventArgs<Feature>(feature));
        }

        public void FeatureFinished(FeatureResult featureResult)
        {
            contextHandler.OnFeatureFinishedEvent();
            if (OnFeatureFinished != null)
                OnFeatureFinished(this, new EventArgs<FeatureResult>(featureResult));
        }

        public void ScenarioStartedEvent(Scenario scenario)
        {
            contextHandler.OnScenarioStartedEvent(scenario);
            if (OnScenarioStarted != null)
                OnScenarioStarted(this, new EventArgs<Scenario>(scenario));
        }

        public void ScenarioFinishedEvent(ScenarioResult result)
        {
            contextHandler.OnScenarioFinishedEvent();
            if (OnScenarioFinished != null)
                OnScenarioFinished(this, new EventArgs<ScenarioResult>(result));
        }

        public void StepStarted(StringStep step)
        {
            contextHandler.OnStepStartedEvent(step);
            if (OnStepStarted != null)
                OnStepStarted(this, new EventArgs<StringStep>(step));
        }

        public void StepFinished(StepResult stepResult)
        {
            contextHandler.OnStepFinishedEvent();
            if (OnStepFinished != null)
                OnStepFinished(this, new EventArgs<StepResult>(stepResult));
        }

        public event EventHandler OnRunStarted;
        public event EventHandler<EventArgs<FeatureResults>> OnRunFinished;
        public event EventHandler<EventArgs<Feature>> OnFeatureStarted;
        public event EventHandler<EventArgs<FeatureResult>> OnFeatureFinished;
        public event EventHandler<EventArgs<Scenario>> OnScenarioStarted;
        public event EventHandler<EventArgs<ScenarioResult>> OnScenarioFinished;
        public event EventHandler<EventArgs<StringStep>> OnStepStarted;
        public event EventHandler<EventArgs<StepResult>> OnStepFinished;
    }
}