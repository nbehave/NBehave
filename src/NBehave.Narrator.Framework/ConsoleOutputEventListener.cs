using System;

namespace NBehave.Narrator.Framework
{
    public class ConsoleOutputEventListener : IEventListener
    {
        private readonly bool _showResults;

        public ConsoleOutputEventListener(bool showResults)
        {
            _showResults = showResults;
        }

        public void FeatureCreated(string feature)
        {
            Console.WriteLine("Feature: " + feature);
        }

        public void FeatureNarrative(string message)
        {
            Console.WriteLine(message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
        }

        public void RunStarted()
        {
        }

        public void RunFinished()
        {
        }

        public void ThemeStarted(string name)
        {
        }

        public void ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult scenarioResult)
        {
            Console.WriteLine("Scenario: " + scenarioResult.ScenarioTitle + (_showResults ? " - " + scenarioResult.Result.ToString().ToUpper() : ""));
            foreach (var stepResult in scenarioResult.ActionStepResults)
                Console.WriteLine(stepResult.StringStep + (_showResults ? " - " + stepResult.Result.ToString().ToUpper() : ""));

        }
    }
}

namespace NBehave.Narrator.Framework
{
}