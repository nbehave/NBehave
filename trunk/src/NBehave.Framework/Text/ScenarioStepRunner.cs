using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Narrator.Framework
{
    public class ScenarioStepRunner
    {
        public IEventListener EventListener { get; set; }
        private readonly Queue<Action> _scenarioEventsToRaise = new Queue<Action>();

        public ScenarioStepRunner()
        {
            EventListener = new NullEventListener();
        }

        public IEnumerable<ScenarioResult> RunScenarios(IEnumerable<ScenarioWithSteps> scenarios)
        {
            var allResults = new List<ScenarioResult>();
            foreach (var scenario in scenarios)
            {
                _scenarioEventsToRaise.Clear();
                IEnumerable<ScenarioResult> scenarioResults = scenario.Run();
                allResults.AddRange(scenarioResults);
            }
            return allResults;
        }
    }
}