using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class ResultPublisher : IDisposable
    {
        private readonly IRemoteTaskServer _server;
        private readonly ITinyMessengerHub _hub;
        readonly Dictionary<string, List<ScenarioResult>> _featureResults = new Dictionary<string, List<ScenarioResult>>();
        private readonly TinyMessageSubscriptionToken _featureStartedEventSubscription;
        private readonly TinyMessageSubscriptionToken _scenarioResultEventSubscription;
        private bool _disposed;

        public ResultPublisher(IRemoteTaskServer server, ITinyMessengerHub hub)
        {
            _server = server;
            _hub = hub;
            var featureResult = new List<ScenarioResult>();
            _featureStartedEventSubscription = _hub.Subscribe<FeatureStartedEvent>(_ =>
            {
                featureResult = new List<ScenarioResult>();
                _featureResults.Add(_.Content, featureResult);
            });
            _scenarioResultEventSubscription = _hub.Subscribe<ScenarioResultEvent>(_ => featureResult.Add(_.Content));
        }

        public void PublishResults(IEnumerable<NBehaveFeatureTask> tasks)
        {
            foreach (var task in tasks)
            {
                var scenarioResults = FindScenarioResultsForFeatureTask(task);
                PublishFeatureResults(scenarioResults, task);
            }
        }

        private IEnumerable<ScenarioResult> FindScenarioResultsForFeatureTask(NBehaveFeatureTask task)
        {
            List<ScenarioResult> featureResult;
            if (_featureResults.TryGetValue(task.FeatureTitle, out featureResult))
                return featureResult;
            return new List<ScenarioResult>();
        }

        private void PublishFeatureResults(IEnumerable<ScenarioResult> results, NBehaveFeatureTask task)
        {
            _server.TaskProgress(task, "");
            PublishTaskResult(task, results);
        }

        private void PublishTaskResult(RemoteTask task, IEnumerable<ScenarioResult> results)
        {
            var taskResult = GetTaskResult(results);
            string taskResultMessage = "";
            if (taskResult == TaskResult.Skipped)
                taskResultMessage = "Skipped";
            if (taskResult == TaskResult.Inconclusive)
            {
                taskResultMessage = "Pending";
                _server.TaskExplain(task, "See pending step(s) for more information");
            }
            if (taskResult == TaskResult.Error)
            {
                var failure =  results.First(_ => _.Result is Failed).Result as Failed;
                taskResultMessage = failure.Exception.Message;
                var te = new TaskException(failure.Exception);
                _server.TaskException(task, new[] { te });
            }
            _server.TaskFinished(task, taskResultMessage, taskResult);
        }

        private TaskResult GetTaskResult(IEnumerable<ScenarioResult> results)
        {
            var taskResult = (results.Any()) ? TaskResult.Skipped : TaskResult.Exception;
            taskResult = (results.Any(_ => _.Result is Passed)) ? TaskResult.Success : taskResult;
            taskResult = (results.Any(_ => _.Result is Pending)) ? TaskResult.Inconclusive : taskResult;
            taskResult = (results.Any(_ => _.Result is Failed)) ? TaskResult.Error : taskResult;
            return taskResult;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _hub.Unsubscribe<FeatureStartedEvent>(_featureStartedEventSubscription);
                _hub.Unsubscribe<ScenarioResultEvent>(_scenarioResultEventSubscription);
            }
        }
    }
}