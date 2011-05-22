// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryWriter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the SummaryWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SummaryWriter
    {
        private readonly TextWriter _writer;

        public SummaryWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteCompleteSummary(List<ScenarioResult> results)
        {
            var featureResults = GetFeatureResult(results);
            WriteSummaryResults(featureResults);
            WriteFailures(featureResults);
        }

        public void WriteSummaryResults(FeatureResults featureResults)
        {
            _writer.WriteLine(
                "Scenarios run: {0}, Failures: {1}, Pending: {2}",
                featureResults.NumberOfScenariosFound,
                featureResults.NumberOfFailingScenarios,
                featureResults.NumberOfPendingScenarios);
            
            var actionSteps = CountActionSteps(featureResults);
            var failedSteps = CountFailedActionSteps(featureResults);
            var pendingSteps = CountPendingActionSteps(featureResults);
            _writer.WriteLine("Steps {0}, failed {1}, pending {2}", actionSteps, failedSteps, pendingSteps);
        }

        public void WriteFailures(FeatureResults results)
        {
            if (results.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Failures:");
                var failureNumber = 1;

                foreach (var result in results.ScenarioResults)
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

        public void WritePending(FeatureResults results)
        {
            if (results.NumberOfPendingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Pending:");
                var pendingNumber = 1;

                foreach (var result in results.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Pending))
                    {
                        _writer.WriteLine(
                            "{0}) {1} ({2}):\n{3}",
                            pendingNumber,
                            result.FeatureTitle,
                            result.ScenarioTitle,
                            result.Message);
                        pendingNumber++;
                    }
                }
            }
        }

        public void WriteSeparator()
        {
            _writer.WriteLine(string.Empty);
        }

        private FeatureResults GetFeatureResult(IEnumerable<ScenarioResult> results)
        {
            var featureResults = new FeatureResults(this);
            foreach (var result in results)
            {
                featureResults.AddResult(result);
            }

            return featureResults;
        }

        private int CountActionSteps(FeatureResults featureResults)
        {
            return CountPassedActionSteps(featureResults) +
                CountPendingActionSteps(featureResults) +
                CountFailedActionSteps(featureResults);
        }

        private int CountPassedActionSteps(FeatureResults featureResults)
        {
            return CountActionStepsOfType(featureResults, typeof(Passed));
        }

        private int CountFailedActionSteps(FeatureResults featureResults)
        {
            return CountActionStepsOfType(featureResults, typeof(Failed));
        }

        private int CountPendingActionSteps(FeatureResults featureResults)
        {
            return CountActionStepsOfType(featureResults, typeof(Pending));
        }

        private int CountActionStepsOfType(FeatureResults featureResults, Type typeOfStep)
        {
            var sum = 0;
            foreach (var result in featureResults.ScenarioResults)
            {
                var count = from r in result.ActionStepResults
                              where r.Result.GetType() == typeOfStep
                              select r;
                sum += count.Count();
            }

            return sum;
        }
    }
}