using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunnerListener : EventListener
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

        private readonly List<ScenarioResult> _scenarioResults = new List<ScenarioResult>();

        public NBehaveTaskRunnerListener(IEnumerable<TaskExecutionNode> nodes, IRemoteTaskServer server)
        {
            _server = server;
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

        public IEnumerable<ScenarioResult> ScenarioResults { get { return _scenarioResults; } }


        public override void ScenarioResult(ScenarioResult result)
        {
            _scenarioResults.Add(result);
            List<TaskState> nodes;
            if (_nodes.TryGetValue(typeof(NBehaveScenarioTask), out nodes) == false)
                return;
            var scenario = nodes
                .FirstOrDefault(_ => ((NBehaveScenarioTask)_.Task).Scenario == result.ScenarioTitle && _.State == SignalState.NotStarted);
            if (scenario == null)
                return;

            NotifyResharperOfStepResults(result);
            NotifyResharperOfScenarioResult(result, scenario);
        }

        private void NotifyResharperOfScenarioResult(ScenarioResult result, TaskState scenario)
        {
            NotifyResharperOfTaskResult(result.Result, result.Message, scenario);
        }

        private void NotifyResharperOfStepResults(ScenarioResult result)
        {
            foreach (var step in result.ActionStepResults)
            {
                List<TaskState> nodes;
                if (_nodes.TryGetValue(typeof(NBehaveStepTask), out nodes) == false)
                    continue;
                TaskState node = nodes.FirstOrDefault(_ => ((NBehaveStepTask)_.Task).Scenario == result.ScenarioTitle && _.State == SignalState.NotStarted);
                if (node == null)
                    continue;

                NotifyResharperOfTaskResult(step.Result, step.Message, node);
            }
        }

        private void NotifyResharperOfTaskResult(Result result, string resultMessage, TaskState scenario)
        {
            TaskResult taskResult = GetTaskResult(result);
            _server.TaskStarting(scenario.Task);
            scenario.State = SignalState.Started;
            if (taskResult == TaskResult.Error)
                _server.TaskException(scenario.Task, new[] { new TaskException(((Failed)result).Exception) });
            if (taskResult == TaskResult.Skipped)
                _server.TaskExplain(scenario.Task, "TODO: have to raise this after all features are finished");
            scenario.State = SignalState.Finished;
            _server.TaskFinished(scenario.Task, resultMessage, taskResult);
        }

        private TaskResult GetTaskResult(Result result)
        {
            if (result is Passed)
                return TaskResult.Success;
            if (result is Failed)
                return TaskResult.Error;
            if (result is Pending)
                return TaskResult.Skipped;
            return TaskResult.Inconclusive;
        }
    }
}