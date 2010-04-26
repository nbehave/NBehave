using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.Narrator.Framework
{
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
            WritePending(featureResults);
        }

        public void WriteSummaryResults(FeatureResults featureResults)
        {
            _writer.WriteLine("Scenarios run: {0}, Failures: {1}, Pending: {2}", featureResults.NumberOfScenariosFound,
                              featureResults.NumberOfFailingScenarios, featureResults.NumberOfPendingScenarios);
            int actionSteps = CountActionSteps(featureResults);
            int failedSteps = CountFailedActionSteps(featureResults);
            int pendingSteps = CountPendingActionSteps(featureResults);
            _writer.WriteLine("Steps {0}, failed {1}, pending {2}", actionSteps, failedSteps, pendingSteps);
        }


        public void WriteFailures(FeatureResults results)
        {
            if (results.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Failures:");
                int failureNumber = 1;

                foreach (ScenarioResult result in results.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Failed))
                    {
                        _writer.WriteLine("{0}) {1} ({2}) FAILED", failureNumber, result.FeatureTitle,
                                          result.ScenarioTitle);
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
                int pendingNumber = 1;

                foreach (ScenarioResult result in results.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Pending))
                    {
                        _writer.WriteLine("{0}) {1} ({2}):\n{3}", pendingNumber, result.FeatureTitle,
                                          result.ScenarioTitle, result.Message);
                        pendingNumber++;
                    }
                }
            }
        }

        public void WriteSeparator()
        {
            _writer.WriteLine("");
        }

        private FeatureResults GetFeatureResult(IEnumerable<ScenarioResult> results)
        {
            var featureResults = new FeatureResults();
            foreach (var result in results)
                featureResults.AddResult(result);
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
            int sum = 0;
            foreach (var result in featureResults.ScenarioResults)
            {
                var toCount = from r in result.ActionStepResults
                              where r.Result.GetType() == typeOfStep
                              select r;
                sum += toCount.Count();
            }
            return sum;
        }
    }
}