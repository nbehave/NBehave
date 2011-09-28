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

        public override void ScenarioFinished(ScenarioResult scenarioResult)
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
            if (stepResult.Result is Pending && (stepResult.Result is Skipped == false))
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

            var ex = new ExampleTableFormatter();
            var columns = "Examples:" + Environment.NewLine + "\t" + ex.TableHeader(scenarioExampleResult.Examples);
            WriteColorString(columns, ConsoleColor.Gray);

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            var rows = ex.TableRows(scenarioExampleResult.Examples);
            for (int i = 0; i < rows.Length; i++)
            {
                WriteColorString("\t" + rows[i],GetColorForResult(scenarioResults[i].Result));
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