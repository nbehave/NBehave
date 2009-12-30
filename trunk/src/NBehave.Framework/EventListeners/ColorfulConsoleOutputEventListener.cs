using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class ColorfulConsoleOutputEventListener : IEventListener
    {
        private List<ScenarioResult> _allResults = new List<ScenarioResult>();

        public void RunStarted()
        {
            _allResults = new List<ScenarioResult>();
        }

        public void FeatureCreated(string feature)
        {
            if (string.IsNullOrEmpty(feature))
            {
                Console.WriteLine();
                return;
            }
            WriteColorString("Feature: " + feature, ConsoleColor.Cyan);
        }

        public void FeatureNarrative(string narrative)
        {
            if (string.IsNullOrEmpty(narrative))
                return; 
            WriteColorString(narrative, ConsoleColor.DarkCyan);
        }

        public void ScenarioCreated(string scenarioTitle)
        { }

        public void ScenarioMessageAdded(string message)
        {
        }

        public void RunFinished()
        {
            Console.WriteLine("");
            Console.ResetColor();
            var failureText = new StringBuilder("-----------------------------------------" + Environment.NewLine);
            foreach (var failedActionStepResult in _allResults)
            {
                if (failedActionStepResult.Result is Failed)
                {
                    failureText.AppendLine(string.Format("Feature: {0}", failedActionStepResult.FeatureTitle));
                    failureText.AppendLine(string.Format("  Scenario: {0}", failedActionStepResult.ScenarioTitle));
                    foreach (var actionStepResult in failedActionStepResult.ActionStepResults)
                    {
                        if (actionStepResult.Result is Failed)
                        {
                            failureText.AppendLine(actionStepResult.ActionStep + " - " + actionStepResult.Result);
                            failureText.AppendLine(failedActionStepResult.Message);
                            failureText.AppendLine((failedActionStepResult.Result as Failed).Exception.StackTrace);
                        }
                    }
                }
            }
        }

        public void ThemeStarted(string name)
        { }

        public void ThemeFinished()
        { }

        public void ScenarioResult(ScenarioResult scenarioResult)
        {
            WriteColorString("Scenario: " + scenarioResult.ScenarioTitle + " - " + scenarioResult.Result.ToString().ToUpper(), GetColorForResult(scenarioResult.Result));
            _allResults.Add(scenarioResult);
            foreach (var stepResult in scenarioResult.ActionStepResults)
                WriteColorString(stepResult.ActionStep + " - " + stepResult.Result.ToString().ToUpper(), GetColorForResult(stepResult.Result));
        }

        private void WriteColorString(string text, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }

        private ConsoleColor GetColorForResult(Result result)
        {
            if (result is Passed)
                return ConsoleColor.Green;
            if (result is Failed)
                return ConsoleColor.Red;
            if (result is Pending)
                return ConsoleColor.Yellow;

            return ConsoleColor.Gray;
        }
    }
}