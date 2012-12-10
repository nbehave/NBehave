// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryWriter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the SummaryWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Domain;

namespace NBehave.EventListeners
{
    public class SummaryWriter
    {
        private readonly TextWriter _writer;

        public SummaryWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteCompleteSummary(IEnumerable<ScenarioResult> results)
        {
            var featureResult = GetFeatureResult(results);
            WriteSummaryResults(featureResult);
            WriteFailures(featureResult);
        }

        public void WriteSummaryResults(FeatureResult featureResult)
        {
            _writer.WriteLine(
                "Scenarios run: {0}, Failures: {1}, Pending: {2}",
                featureResult.NumberOfScenariosFound,
                featureResult.NumberOfFailingScenarios,
                featureResult.NumberOfPendingScenarios);

            var actionSteps = CountActionSteps(featureResult);
            var failedSteps = CountFailedActionSteps(featureResult);
            var pendingSteps = CountPendingActionSteps(featureResult);
            _writer.WriteLine("Steps {0}, failed {1}, pending {2}", actionSteps, failedSteps, pendingSteps);
        }

        private void WriteFailures(FeatureResult featureResult)
        {
            if (featureResult.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Failures:");
                var failureNumber = 1;

                foreach (var result in featureResult.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Failed))
                    {
                        _writer.WriteLine(
                            "{0}) {1} ({2}) FAILED", failureNumber, result.FeatureTitle, result.ScenarioTitle);
                        _writer.WriteLine("  {0}", result.Message);
                        _writer.WriteLine("{0}", result.StackTrace);
                        failureNumber++;
                    }
                }
            }
        }

        private void WriteSeparator()
        {
            _writer.WriteLine(string.Empty);
        }

        private FeatureResult GetFeatureResult(IEnumerable<ScenarioResult> scenarioResults)
        {
            var title = scenarioResults.Select(_ => _.FeatureTitle).FirstOrDefault() ?? "";
            var featureResult = new FeatureResult(title);
            foreach (var result in scenarioResults)
                featureResult.AddResult(result);

            return featureResult;
        }

        private int CountActionSteps(FeatureResult featureResult)
        {
            return CountPassedActionSteps(featureResult) +
                CountPendingActionSteps(featureResult) +
                CountFailedActionSteps(featureResult);
        }

        private int CountPassedActionSteps(FeatureResult featureResult)
        {
            return CountActionStepsOfType(featureResult, r => r is Passed);
        }

        private int CountFailedActionSteps(FeatureResult featureResult)
        {
            return CountActionStepsOfType(featureResult, r => r is Failed);
        }

        private int CountPendingActionSteps(FeatureResult featureResult)
        {
            return CountActionStepsOfType(featureResult, r => r is Pending);
        }

        private int CountActionStepsOfType(FeatureResult featureResult, Predicate<Result> isOfType)
        {
            var sum = 0;
            foreach (var result in featureResult.ScenarioResults)
            {
                var count = from r in result.StepResults
                            where isOfType(r.Result)
                            select r;
                sum += count.Count();
            }

            return sum;
        }
    }
}