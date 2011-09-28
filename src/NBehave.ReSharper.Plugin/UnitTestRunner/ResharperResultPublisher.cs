using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

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

        private readonly Dictionary<Type, List<TaskState>> _nodes = new Dictionary<Type, List<TaskState>>();
        private readonly IRemoteTaskServer _server;
        private readonly CodeGenEventListener _codeGeneration;


        public ResharperResultPublisher(IEnumerable<TaskExecutionNode> nodes, IRemoteTaskServer server, CodeGenEventListener codeGeneration)
        {
            _server = server;
            _codeGeneration = codeGeneration;
            AddNodes(nodes);
        }

        private void AddNodes(IEnumerable<TaskExecutionNode> nodes)
        {
            foreach (var task in nodes.AllTasks())
            {
                var nodeType = task.GetType();
                if (_nodes.ContainsKey(nodeType) == false)
                    _nodes.Add(nodeType, new List<TaskState>());
                _nodes[nodeType].Add(new TaskState(task));
            }
        }

        public void Notify(ScenarioResult result)
        {
            List<TaskState> nodes;
            if (_nodes.TryGetValue(typeof(NBehaveScenarioTask), out nodes) == false)
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
            if (_nodes.TryGetValue(typeof(T), out nodes) == false)
                return null;
            TaskState node = GetTaskNodesNotStarted<T>()
                .FirstOrDefault(_ => where((T)_.Task));
            return node;
        }

        private void NotifyResharperOfTaskResult(ScenarioResult scenarioResult, StepResult result, TaskState taskState)
        {
            TaskResult taskResult = GetTaskResult(result.Result);
            _server.TaskStarting(taskState.Task);
            taskState.State = SignalState.Started;
            if (taskResult == TaskResult.Error)
                _server.TaskException(taskState.Task, new[] { new TaskException(((Failed)result.Result).Exception) });
            if (taskResult == TaskResult.Inconclusive)
            {
                var code = GetCodeForPendingStep(scenarioResult, result);
                var msg = (code == null) ? "" : string.Format("The step can be implemented with:{0}{0}{1}", Environment.NewLine, code.Code);
                _server.TaskExplain(taskState.Task, msg);
            }
            if (taskResult == TaskResult.Skipped)
            {
                _server.TaskExplain(taskState.Task, result.Message);
            }
            taskState.State = SignalState.Finished;
            _server.TaskFinished(taskState.Task, result.Message, taskResult);
        }

        private IEnumerable<TaskState> GetTaskNodesNotStarted<T>()
        {
            if (_nodes.ContainsKey(typeof(T)) == false)
                return new List<TaskState>();
            return _nodes[typeof(T)].Where(_ => _.State == SignalState.NotStarted);
        }

        private CodeGenStep GetCodeForPendingStep(ScenarioResult result, StepResult step)
        {
            return _codeGeneration.PendingSteps
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