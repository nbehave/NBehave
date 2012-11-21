using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Extensions;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class ResharperResultPublisher
    {
        private enum SignalState
        {
            NotStarted,
            Started,
            Finished
        }

        private class TaskState
        {
            public RemoteTask Task { get; private set; }
            public SignalState State { get; set; }

            public TaskState(RemoteTask task)
            {
                Task = task;
                State = SignalState.NotStarted;
            }
        }

        private readonly Dictionary<Type, List<TaskState>> nodes = new Dictionary<Type, List<TaskState>>();
        private readonly IRemoteTaskServer server;
        private readonly CodeGenEventListener codeGeneration;


        public ResharperResultPublisher(IEnumerable<TaskExecutionNode> nodes, IRemoteTaskServer server, CodeGenEventListener codeGeneration)
        {
            this.server = server;
            this.codeGeneration = codeGeneration;
            AddNodes(nodes);
        }

        private void AddNodes(IEnumerable<TaskExecutionNode> taskExecutionNodes)
        {
            foreach (var task in taskExecutionNodes.AllTasks())
            {
                var nodeType = task.GetType();
                if (nodes.ContainsKey(nodeType) == false)
                    nodes.Add(nodeType, new List<TaskState>());
                nodes[nodeType].Add(new TaskState(task));
            }
        }

        public void Notify(FeatureResult result)
        {
            List<TaskState> nodes;
            if (this.nodes.TryGetValue(typeof(NBehaveFeatureTask), out nodes) == false)
                return;
            var taskState = nodes.FirstOrDefault(_ => ((NBehaveFeatureTask)_.Task).FeatureTitle == result.FeatureTitle);
            if (taskState == null)
                return;
            server.TaskProgress(taskState.Task, "");
            PublishTaskResult(taskState.Task, result);
        }

        private void PublishTaskResult(RemoteTask task, FeatureResult result)
        {
            var taskResult = GetFeatureTaskResult(result);
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
                var failure = result.ScenarioResults.First(_ => _.Result is Failed).Result as Failed;
                taskResultMessage = failure.Exception.Message;
                var te = new TaskException(failure.Exception);
                server.TaskException(task, new[] { te });
            }
            server.TaskFinished(task, taskResultMessage, taskResult);
        }

        private TaskResult GetFeatureTaskResult(FeatureResult result)
        {
            var results = result.ScenarioResults;
            var taskResult = (results.Any()) ? TaskResult.Skipped : TaskResult.Exception;
            taskResult = (results.Any(_ => _.Result is Passed)) ? TaskResult.Success : taskResult;
            taskResult = (results.Any(_ => _.Result is Pending)) ? TaskResult.Inconclusive : taskResult;
            taskResult = (results.Any(_ => _.Result is Failed)) ? TaskResult.Error : taskResult;
            return taskResult;
        }

        public void Notify(ScenarioResult result)
        {
            List<TaskState> nodes;
            if (this.nodes.TryGetValue(typeof(NBehaveScenarioTask), out nodes) == false)
                return;
            var scenario = GetTaskNodesNotStarted<NBehaveScenarioTask>()
                .FirstOrDefault(_ => ((NBehaveScenarioTask)_.Task).Scenario == result.ScenarioTitle);
            if (scenario == null)
                return;

            NotifyResharperOfStepResults(result);
            NotifyResharperOfScenarioResult(result, scenario);
            NotifyResharperOfBackgroundResult(result);
            NotifyResharperOfExampleResults(result as ScenarioExampleResult);
        }

        private void NotifyResharperOfExampleResults(ScenarioExampleResult result)
        {
            if (result == null)
                return;

            var scenarioResults = result.ExampleResults.ToArray();
            var idx = 0;
            foreach (var example in result.Examples)
            {
                var scenarioResult = scenarioResults[idx++];
                var node = GetTaskStateFor<NBehaveExampleTask>(_ => _.Scenario == result.ScenarioTitle && _.Example == example.ToString());
                if (node == null)
                    continue;

                NotifyResharperOfTaskResult(result, CreateStepResult(example, scenarioResult), node);
            }
        }

        private static StepResult CreateStepResult(Example example, ScenarioResult scenarioResult)
        {
            var source = scenarioResult.StepResults.First().StringStep.Source;
            return new StepResult(new StringStep(example.ColumnValuesToString(), source), scenarioResult.Result);
        }

        private void NotifyResharperOfBackgroundResult(ScenarioResult scenarioResult)
        {
            if (scenarioResult.HasBackgroundResults() == false)
                return;

            var backgroundTasks = GetTaskNodesNotStarted<NBehaveBackgroundTask>()
                .Where(_ => ((NBehaveBackgroundTask)_.Task).Scenario == scenarioResult.ScenarioTitle);
            var backgroundResult = GetBackgroundStepResult(scenarioResult);
            foreach (var backgroundTask in backgroundTasks)
            {
                backgroundTask.State = SignalState.Finished;
                NotifyResharperOfTaskResult(scenarioResult, backgroundResult, backgroundTask);
            }
        }

        private BackgroundStepResult GetBackgroundStepResult(ScenarioResult scenarioResult)
        {
            var results = scenarioResult.StepResults.Where(_ => _ is BackgroundStepResult).Cast<BackgroundStepResult>();
            var backgroundResult = results.First();
            foreach (var result in results)
            {
                backgroundResult = (result.Result is Failed) ? result : backgroundResult;
                backgroundResult = (result.Result is Pending && backgroundResult is Passed) ? result : backgroundResult;
            }
            return backgroundResult;
        }

        private void NotifyResharperOfScenarioResult(ScenarioResult result, TaskState scenario)
        {
            var stepResult = new StepResult(result.ScenarioTitle.AsStringStep(""), result.Result);
            NotifyResharperOfTaskResult(result, stepResult, scenario);
        }

        private void NotifyResharperOfStepResults(ScenarioResult result)
        {
            foreach (var step in result.StepResults)
            {
                var node = GetTaskStateFor<NBehaveStepTask>(_ => _.Scenario == result.ScenarioTitle && _.Step == step.StringStep.Step);
                if (node == null)
                    continue;

                NotifyResharperOfTaskResult(result, step, node);
            }
        }

        private TaskState GetTaskStateFor<T>(Predicate<T> where) where T : NBehaveRemoteTask
        {
            List<TaskState> nodes;
            if (this.nodes.TryGetValue(typeof(T), out nodes) == false)
                return null;
            TaskState node = GetTaskNodesNotStarted<T>()
                .FirstOrDefault(_ => where((T)_.Task));
            return node;
        }

        private void NotifyResharperOfTaskResult(ScenarioResult scenarioResult, StepResult result, TaskState taskState)
        {
            TaskResult taskResult = GetTaskResult(result.Result);
            server.TaskStarting(taskState.Task);
            taskState.State = SignalState.Started;
            if (taskResult == TaskResult.Error)
                server.TaskException(taskState.Task, new[] { new TaskException(((Failed)result.Result).Exception) });
            if (result.StringStep is StringTableStep)
            {
                var tableStep = result.StringStep as StringTableStep;
                var x = new ExampleTableFormatter();
                var msg = x.TableHeader(tableStep.TableSteps) + Environment.NewLine + string.Join(Environment.NewLine, x.TableRows(tableStep.TableSteps));
                server.TaskOutput(taskState.Task, msg, TaskOutputType.STDOUT);
            }
            if (taskResult == TaskResult.Inconclusive)
            {
#if RESHARPER_71
                Func<RemoteTask, string, bool> f = (task, msg) => server.TaskOutput(task, msg, TaskOutputType.STDOUT);
                ExplainPendingStep(scenarioResult, result, taskState, f);
#else
                ExplainPendingStep(scenarioResult, result, taskState, server.TaskExplain);
#endif
            }
            if (taskResult == TaskResult.Skipped)
            {
                server.TaskExplain(taskState.Task, result.Message);
            }
            taskState.State = SignalState.Finished;
            server.TaskFinished(taskState.Task, result.Message, taskResult);
        }

        private void ExplainPendingStep(ScenarioResult scenarioResult, StepResult result, TaskState taskState, Func<RemoteTask, string, bool> notifier)
        {
            var msg = GetPendingStepImplementationSuggestion(scenarioResult, result);
            notifier(taskState.Task, msg);
        }

        private string GetPendingStepImplementationSuggestion(ScenarioResult scenarioResult, StepResult result)
        {
            var code = GetCodeForPendingStep(scenarioResult, result);
            var msg = (code == null) ? "" : string.Format("The step can be implemented with:{0}{0}{1}", Environment.NewLine, code.Code);
            return msg;
        }

        private IEnumerable<TaskState> GetTaskNodesNotStarted<T>()
        {
            if (nodes.ContainsKey(typeof(T)) == false)
                return new List<TaskState>();
            return nodes[typeof(T)].Where(_ => _.State == SignalState.NotStarted);
        }

        private CodeGenStep GetCodeForPendingStep(ScenarioResult result, StepResult step)
        {
            return codeGeneration.PendingSteps
                .FirstOrDefault(_ => _.Feature == result.FeatureTitle
                                     && _.Scenario == result.ScenarioTitle
                                     && _.Step == step.StringStep.Step);
        }

        private TaskResult GetTaskResult(Result result)
        {
            if (result is Passed)
                return TaskResult.Success;
            if (result is Failed)
                return TaskResult.Error;
            if (result is Pending)
                return TaskResult.Inconclusive;
            return TaskResult.Skipped;
        }

    }
}