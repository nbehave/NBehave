// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the OutputEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Domain;

namespace NBehave.EventListeners
{
    public class OutputEventListener : EventListener
    {
        private readonly List<ScenarioResult> allResults = new List<ScenarioResult>();

        private readonly IOutputWriter writer;

        public OutputEventListener(IOutputWriter writer)
        {
            this.writer = writer;
        }

        public override void FeatureStarted(Feature feature)
        {
            if (feature == null || string.IsNullOrEmpty(feature.Title))
            {
                writer.WriteLine();
                return;
            }

            writer.WriteColorString("Feature: " + feature.Title, ConsoleColor.Cyan);
            writer.WriteColorString(feature.Narrative, ConsoleColor.DarkCyan);
        }

        public override void RunStarted()
        { }

        public override void RunFinished()
        {
            writer.WriteLine(string.Empty);
            writer.ResetColor();
            var failureText = new StringBuilder("-----------------------------------------" + Environment.NewLine);
            foreach (var failedActionStepResult in allResults)
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
            writer.WriteColorString("Scenario: " + scenarioResult.ScenarioTitle + " - " + scenarioResult.Result.ToString().ToUpper(), GetColorForResult(scenarioResult.Result));
            allResults.Add(scenarioResult);
            foreach (var stepResult in scenarioResult.StepResults)
            {
                writer.WriteColorString("  " + stepResult.StringStep.Step + " - " + TypeAsString(stepResult), GetColorForResult(stepResult.Result));
                WriteTableSteps(stepResult);
            }

            DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
        }

        private void WriteTableSteps(StepResult stepResult)
        {
            if (stepResult.StringStep is StringTableStep == false)
                return;

            var stringTableStep = stepResult.StringStep as StringTableStep;
            var heeader = stringTableStep.TableSteps.First();
            writer.WriteColorString("   " + heeader.ColumnNamesToString(), GetColorForResult(stepResult.Result));
            foreach (var tableStep in stringTableStep.TableSteps)
                writer.WriteColorString("   " + tableStep.ColumnValuesToString(), GetColorForResult(stepResult.Result));
        }

        private string TypeAsString(StepResult stepResult)
        {
            if (stepResult.Result is Pending && (stepResult.Result is Skipped == false))
                return "PENDING";
            return stepResult.Result.ToString().ToUpper();
        }

        private void WriteBackground(ScenarioResult scenarioResult)
        {
            var backgroundSteps = scenarioResult.StepResults.Where(_ => _ is BackgroundStepResult).Cast<BackgroundStepResult>().ToList();
            if (backgroundSteps.Any())
                new BackgroundWriter(writer.Out).Write(backgroundSteps);
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(writer.Out);
            summaryWriter.WriteCompleteSummary(allResults);
        }

        private void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
        {
            if (scenarioExampleResult == null)
                return;

            var ex = new ExampleTableFormatter();
            var columns = "Examples:" + Environment.NewLine + "\t" + ex.TableHeader(scenarioExampleResult.Examples);
            writer.WriteColorString(columns, ConsoleColor.Gray);

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            var rows = ex.TableRows(scenarioExampleResult.Examples);
            for (int i = 0; i < rows.Length; i++)
                writer.WriteColorString("\t" + rows[i], GetColorForResult(scenarioResults[i].Result));
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