// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioStepRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioStepRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioStepRunner
    {
        private readonly ITinyMessengerHub _hub;

        private Feature _lastFeature;

        public ScenarioStepRunner(ITinyMessengerHub hub)
        {
            _hub = hub;
        }

        public IEnumerable<ScenarioResult> Run(IEnumerable<ScenarioWithSteps> scenarios)
        {
            var allResults = new List<ScenarioResult>();

            foreach (var scenario in scenarios)
            {
                if (scenario.Feature != _lastFeature)
                {
                    NewFeature(scenario);
                }

                var scenarioResults = scenario.Run();
                RaiseFeatureResultsEvent(scenarioResults);
                allResults.Add(scenarioResults);
            }

            return allResults;
        }

        private void NewFeature(ScenarioWithSteps scenario)
        {
            _lastFeature = scenario.Feature;
            _lastFeature.RaiseFeatureCreated();
        }

        private void RaiseFeatureResultsEvent(ScenarioResult scenarioResult)
        {
            _hub.Publish(new ScenarioResultMessage(this, scenarioResult));
        }
    }
}