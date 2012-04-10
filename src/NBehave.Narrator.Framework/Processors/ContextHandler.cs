using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Processors
{
    public class ContextHandler : IContextHandler
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private readonly StepContext stepContext;

        private readonly List<string> tags = new List<string>();
        private readonly List<string> featureTags = new List<string>();

        public ContextHandler(FeatureContext featureContext, ScenarioContext scenarioContext, StepContext stepContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.stepContext = stepContext;
        }

        private readonly char[] at = new[] { '@' };
        public void OnParsedTagEvent(string tag)
        {
            tags.Add(tag.TrimStart(at));
        }

        public void OnFeatureStartedEvent(Feature feature)
        {
            featureContext.ClearTags();
            featureContext.Feature = feature;
            featureTags.AddRange(tags);
            featureContext.AddTags(tags);
        }

        public void OnFeatureFinishedEvent()
        {
            DisposeContextValues(featureContext);
            featureTags.Clear();
            tags.Clear();
        }

        public void OnScenarioStartedEvent(Scenario scenario)
        {
            scenarioContext.ClearTags();
            scenarioContext.Scenario = scenario;
            scenarioContext.AddTags(tags);
        }

        public void OnScenarioFinishedEvent()
        {
            DisposeContextValues(scenarioContext);
            tags.Clear();
            tags.AddRange(featureTags);
        }

        public void OnStepStartedEvent(StringStep step)
        {
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