using System;

namespace NBehave.Internal
{
    public interface IRunContextEvents
    {
        event EventHandler OnRunStarted;
        event EventHandler<EventArgs<FeatureResults>> OnRunFinished;
        event EventHandler<EventArgs<Feature>> OnFeatureStarted;
        event EventHandler<EventArgs<FeatureResult>> OnFeatureFinished;
        event EventHandler<EventArgs<Scenario>> OnScenarioStarted;
        event EventHandler<EventArgs<ScenarioResult>> OnScenarioFinished;
        event EventHandler<EventArgs<StringStep>> OnStepStarted;
        event EventHandler<EventArgs<StepResult>> OnStepFinished;
    }
}