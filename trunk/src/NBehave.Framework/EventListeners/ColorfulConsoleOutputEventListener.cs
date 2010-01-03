using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class ColorfulConsoleOutputEventListener : IEventListener
    {
        private List<ScenarioResult> _allResults = new List<ScenarioResult>();

        void IEventListener.RunStarted()
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

        void IEventListener.FeatureNarrative(string narrative)
        {
            if (string.IsNullOrEmpty(narrative))
                return;
            WriteColorString(narrative, ConsoleColor.DarkCyan);
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        { }

        void IEventListener.RunFinished()
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
                            failureText.AppendLine(actionStepResult.StringStep + " - " + actionStepResult.Result);
                            failureText.AppendLine(failedActionStepResult.Message);
                            failureText.AppendLine((failedActionStepResult.Result as Failed).Exception.StackTrace);
                        }
                    }
                }
            }
            WriteSummary();
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(Console.Out);
            summaryWriter.WriteCompleteSummary(_allResults);
        }

        void IEventListener.ThemeStarted(string name)
        { }

        void IEventListener.ThemeFinished()
        { }

        void IEventListener.ScenarioResult(ScenarioResult scenarioResult)
        {
            WriteColorString("Scenario: " + scenarioResult.ScenarioTitle + " - " + scenarioResult.Result.ToString().ToUpper(), GetColorForResult(scenarioResult.Result));
            _allResults.Add(scenarioResult);
            foreach (var stepResult in scenarioResult.ActionStepResults)
                WriteColorString(stepResult.StringStep + " - " + stepResult.Result.ToString().ToUpper(), GetColorForResult(stepResult.Result));
      
            DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
        }


        private void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
        {
            if (scenarioExampleResult == null)
                return;

            string columns = "Examples:" + Environment.NewLine+ "|";
            foreach (var columnName in scenarioExampleResult.Examples.First().ColumnNames)
            {
                columns += columnName + "|";
            }
            WriteColorString(columns, ConsoleColor.Gray);

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            int idx = 0;
            foreach (var example in scenarioExampleResult.Examples)
            {
                string row = "|";
                foreach (var columnName in example.ColumnNames)
                {
                    row += example.ColumnValues[columnName] + "|";
                }
                WriteColorString(row, GetColorForResult(scenarioResults[idx++].Result));
            }
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