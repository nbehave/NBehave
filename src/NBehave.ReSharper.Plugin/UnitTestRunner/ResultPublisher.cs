using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class ResultPublisher : IDisposable
    {
        readonly IRemoteTaskServer server;
        readonly IRunContextEvents contextEvents;
        readonly Dictionary<string, List<ScenarioResult>> featureResults = new Dictionary<string, List<ScenarioResult>>();
        bool disposed;

        string title = "";
        readonly EventHandler<EventArgs<Feature>> featureStarted;
        readonly EventHandler<EventArgs<FeatureResult>> featureFinished;

        public ResultPublisher(IRemoteTaskServer server, IRunContextEvents contextEvents)
        {
            this.server = server;
            this.contextEvents = contextEvents;
            featureStarted = (s, e) => { title = e.EventInfo.Title; };
            featureFinished = (s, e) => featureResults.Add(title, e.EventInfo.ScenarioResults.ToList()); contextEvents.OnFeatureFinished += featureFinished;
            contextEvents.OnFeatureStarted += featureStarted;
            contextEvents.OnFeatureFinished += featureFinished;
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
            if (featureResults.TryGetValue(task.FeatureTitle, out featureResult))
                return featureResult;
            return new List<ScenarioResult>();
        }

        private void PublishFeatureResults(IEnumerable<ScenarioResult> results, NBehaveFeatureTask task)
        {
            server.TaskProgress(task, "");
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
                server.TaskExplain(task, "See pending step(s) for more information");
            }
            if (taskResult == TaskResult.Error)
            {
                var failure = results.First(_ => _.Result is Failed).Result as Failed;
                taskResultMessage = failure.Exception.Message;
                var te = new TaskException(failure.Exception);
                server.TaskException(task, new[] { te });
            }
            server.TaskFinished(task, taskResultMessage, taskResult);
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
            if (disposing && !disposed)
            {
                disposed = true;
                contextEvents.OnFeatureStarted += featureStarted;
                contextEvents.OnFeatureFinished += featureFinished;
            }
        }
    }
}