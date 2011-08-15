using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

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
        private readonly CodeGenEventListener _codeGeneration;

        public NBehaveTaskRunnerListener(IEnumerable<TaskExecutionNode> nodes,
            IRemoteTaskServer server,
            CodeGenEventListener codeGeneration)
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
            var a = new StepResult(result.ScenarioTitle, result.Result);
            NotifyResharperOfTaskResult(result, a, scenario);
        }

        private void NotifyResharperOfStepResults(ScenarioResult result)
        {
            foreach (var step in result.StepResults)
            {
                List<TaskState> nodes;
                if (_nodes.TryGetValue(typeof(NBehaveStepTask), out nodes) == false)
                    continue;
                TaskState node = nodes.FirstOrDefault(_ => ((NBehaveStepTask)_.Task).Scenario == result.ScenarioTitle
                    && ((NBehaveStepTask)_.Task).Step == step.StringStep
                    && _.State == SignalState.NotStarted);
                if (node == null)
                    continue;

                NotifyResharperOfTaskResult(result, step, node);
            }
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
            if(taskResult == TaskResult.Skipped)
            {
                _server.TaskExplain(taskState.Task, result.Message);
            }
            taskState.State = SignalState.Finished;
            _server.TaskFinished(taskState.Task, result.Message, taskResult);
        }

        private CodeGenStep GetCodeForPendingStep(ScenarioResult result, StepResult step)
        {
            return _codeGeneration.PendingSteps
                .FirstOrDefault(_ => _.Feature == result.FeatureTitle
                                     && _.Scenario == result.ScenarioTitle
                                     && _.Step == step.StringStep);
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