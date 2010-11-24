using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class ScenarioStepRunner
    {
        private readonly IStringStepRunner _stepRunner;
        private Feature _lastFeature;

        public ScenarioStepRunner(IStringStepRunner stepRunner)
        {
            _stepRunner = stepRunner;
        }

        public static event EventHandler<EventArgs<ScenarioResult>> ScenarioResultCreated;

        public IEnumerable<ScenarioResult> Run(IEnumerable<ScenarioWithSteps> scenarios)
        {
            var allResults = new List<ScenarioResult>();

            foreach (var scenario in scenarios)
            {
                if (scenario.Feature != _lastFeature)
                    NewFeature(scenario);
                var scenarioResults = scenario.Run();
                RaiseFeatureResultsEvent(scenarioResults);
                allResults.Add(scenarioResults);
            }
            HandleScenarioEnd();
            return allResults;
        }

        private void HandleScenarioEnd()
        {
            _stepRunner.OnCloseScenario();
        }

        private void NewFeature(ScenarioWithSteps scenario)
        {
            _lastFeature = scenario.Feature;
            _lastFeature.RaiseFeatureCreated();
        }

        private void RaiseFeatureResultsEvent(ScenarioResult scenarioResult)
        {
            if (ScenarioResultCreated == null)
                return;
            var e = new EventArgs<ScenarioResult>(scenarioResult);
            ScenarioResultCreated.Invoke(this, e);
        }
    }
}