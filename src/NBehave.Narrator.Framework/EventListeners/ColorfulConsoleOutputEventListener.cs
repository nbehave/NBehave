// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorfulConsoleOutputEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ColorfulConsoleOutputEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ColorfulConsoleOutputEventListener : EventListener
    {
        private readonly List<ScenarioResult> _allResults = new List<ScenarioResult>();

        public override void FeatureStarted(string feature)
        {
            if (string.IsNullOrEmpty(feature))
            {
                Console.WriteLine();
                return;
            }

            WriteColorString("Feature: " + feature, ConsoleColor.Cyan);
        }

        public override void RunStarted()
        {
        }

        public override void FeatureNarrative(string narrative)
        {
            if (string.IsNullOrEmpty(narrative))
            {
                return;
            }

            WriteColorString(narrative, ConsoleColor.DarkCyan);
        }

        public override void RunFinished()
        {
            Console.WriteLine(string.Empty);
            Console.ResetColor();
            var failureText = new StringBuilder("-----------------------------------------" + Environment.NewLine);
            foreach (var failedActionStepResult in _allResults)
            {
                if (failedActionStepResult.Result is Failed)
                {
                    failureText.AppendLine(string.Format("Feature: {0}", failedActionStepResult.FeatureTitle));
                    failureText.AppendLine(string.Format("  Scenario: {0}", failedActionStepResult.ScenarioTitle));
                    foreach (var actionStepResult in failedActionStepResult.StepResults)
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

        public override void ScenarioResult(ScenarioResult scenarioResult)
        {
            WriteBackground(scenarioResult);
            WriteColorString("Scenario: " + scenarioResult.ScenarioTitle + " - " + scenarioResult.Result.ToString().ToUpper(), GetColorForResult(scenarioResult.Result));
            _allResults.Add(scenarioResult);
            foreach (var stepResult in scenarioResult.StepResults)
            {
                WriteColorString("  " + stepResult.StringStep + " - " + TypeAsString(stepResult),
                    GetColorForResult(stepResult.Result));
            }

            DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
        }

        private static string TypeAsString(StepResult stepResult)
        {
            if (stepResult.Result is Pending)
                return "PENDING";
            return stepResult.Result.ToString().ToUpper();
        }

        private void WriteBackground(ScenarioResult scenarioResult)
        {
            var backgroundSteps = scenarioResult.StepResults.Where(_ => _ is BackgroundStepResult).Cast<BackgroundStepResult>().ToList();
            if (backgroundSteps.Any())
                new BackgroundWriter(Console.Out).Write(backgroundSteps);
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(Console.Out);
            summaryWriter.WriteCompleteSummary(_allResults);
        }

        private void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
        {
            if (scenarioExampleResult == null)
                return;

            var columnSize = CalcColumnSize(scenarioExampleResult);
            var columns = "Examples:" + Environment.NewLine + "\t|";
            foreach (var columnName in scenarioExampleResult.Examples.First().ColumnNames)
                columns += FormatColumnValue(columnSize, columnName.Name, columnName.Name);

            WriteColorString(columns, ConsoleColor.Gray);

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            var idx = 0;
            foreach (var example in scenarioExampleResult.Examples)
            {
                var row = "\t|";
                foreach (var columnName in example.ColumnNames)
                    row += FormatColumnValue(columnSize, columnName.Name, example.ColumnValues[columnName.Name.ToLower()]);

                WriteColorString(row, GetColorForResult(scenarioResults[idx++].Result));
            }
        }

        private string FormatColumnValue(Dictionary<string, int> columnSize, string columnName, string columnValue)
        {
            return string.Format(" {0} |", columnValue.PadRight(columnSize[columnName]));
        }

        private Dictionary<string, int> CalcColumnSize(ScenarioExampleResult result)
        {
            var columns = new Dictionary<string, int>();
            foreach (var example in result.Examples.First().ColumnNames)
                columns.Add(example.Name, example.Name.Length);
            foreach (var example in result.Examples)
            {
                foreach (var columnName in example.ColumnNames)
                {
                    var name = columnName.Name;
                    var valueLength = example.ColumnValues[name].Length;
                    if (columns[name] < valueLength)
                        columns[name] = valueLength;
                }

            }

            return columns;
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
            {
                return ConsoleColor.Green;
            }

            if (result is Failed)
            {
                return ConsoleColor.Red;
            }

            if (result is Pending)
            {
                return ConsoleColor.Yellow;
            }

            return ConsoleColor.Gray;
        }
    }
}