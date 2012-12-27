namespace NBehave.Internal
{
    public interface IRunContext
    {
        void RunStarted();
        void RunFinished(FeatureResults results);
        void FeatureStarted(Feature feature);
        void FeatureFinished(FeatureResult featureResult);
        void ScenarioStartedEvent(Scenario scenario);
        void ScenarioFinishedEvent(ScenarioResult result);
        void StepStarted(StringStep step);
        void StepFinished(StepResult stepResult);
    }
}