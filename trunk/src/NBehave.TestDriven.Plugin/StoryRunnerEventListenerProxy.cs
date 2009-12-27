using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    public class StoryRunnerEventListenerProxy : IEventListener
    {
        private readonly ITestListener _listener;
        private readonly List<ScenarioResult> _allResults = new List<ScenarioResult>();

        public StoryRunnerEventListenerProxy(ITestListener listener)
        {
            _listener = listener;
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
            _listener.WriteLine(string.Format("Theme : {0}", name), Category.Output);
        }

        void IEventListener.StoryCreated(string story)
        {
            _listener.WriteLine("\tStory: " + story, Category.Output);
        }

        void IEventListener.StoryMessageAdded(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        void IEventListener.ScenarioResult(ScenarioResult result)
        {
            _allResults.Add(result);
            _listener.WriteLine(string.Format("\t\tScenario: {0} - {1}", result.ScenarioTitle, result.Result), Category.Info);
        }

        void IEventListener.RunFinished()
        {
            var summary = new TestResult();
            summary.TotalTests = _allResults.Count;
            summary.State = TestState.Passed;
            summary.Message = CreateSummary();
            _listener.TestFinished(summary);
        }

        private string CreateSummary()
        {
            var features = _allResults.Select(f => f.FeatureTitle).Distinct().Count();
            var allScenarios = _allResults.Count();
            var pendingScenarios = _allResults.Select(s => s.Result is Pending).Count();
            var failedScenarios = _allResults.Select(s => s.Result is Failed).Count();
            var allSteps = GetAllSteps();
            var pendingSteps = allSteps.Select(f => f.Result is Pending).Count();
            var failedSteps = allSteps.Select(f => f.Result is Failed).Count();
            string summary = string.Format("Features: {1}{0}Scenarios: {2}, scenarios pending: {3}, scenarios failed: {4}{0}Steps: {5}, steps pending: {6}, steps failed: {7}",
                                           Environment.NewLine, features, allScenarios, pendingScenarios, failedScenarios, allSteps.Count, pendingSteps, failedSteps);
            return summary;
        }

        private List<ActionStepResult> GetAllSteps()
        {
            var steps = new List<ActionStepResult>();

            foreach (var result in _allResults)
                steps.AddRange(result.ActionStepResults);
            return steps;
        }

        private TestState GetTestResultState(Result result)
        {
            if (result.GetType() == typeof(Passed))
                return TestState.Passed;
            if (result.GetType() == typeof(Failed))
                return TestState.Failed;
            if (result.GetType() == typeof(Pending))
                return TestState.Ignored;
            throw new NotSupportedException(string.Format("Result {0} isn't supported.", result));
        }

    }
}
