using System;

namespace NBehave.Internal
{
    public class ContextHandler : IContextHandler
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private readonly StepContext stepContext;

        public ContextHandler(FeatureContext featureContext, ScenarioContext scenarioContext, StepContext stepContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.stepContext = stepContext;
        }

        private readonly char[] at = new[] { '@' };

        public void OnFeatureStartedEvent(Feature feature)
        {
            featureContext.ClearTags();
            featureContext.Feature = feature;
            featureContext.AddTags(feature.Tags);
        }

        public void OnFeatureFinishedEvent()
        {
            DisposeContextValues(featureContext);
        }

        public void OnScenarioStartedEvent(Scenario scenario)
        {
            scenarioContext.ClearTags();
            scenarioContext.Scenario = scenario;
            scenarioContext.AddTags(scenario.Tags);
        }

        public void OnScenarioFinishedEvent()
        {
            DisposeContextValues(scenarioContext);
        }

        public void OnStepStartedEvent(StringStep step)
        {
            stepContext.ClearTags();
            stepContext.AddTags(scenarioContext.Tags);
            stepContext.StringStep = step;
        }

        public void OnStepFinishedEvent()
        { }

        private void DisposeContextValues(NBehaveContext context)
        {
            foreach (var value in context.Values)
            {
                var d = value as IDisposable;
                if (d != null)
                    d.Dispose();
            }
            context.Clear();
        }
    }
}