using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.ReSharper.Plugin.UnitTestProvider;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = TestProvider.NBehaveId;

        public NBehaveTaskRunner(IRemoteTaskServer server)
            : base(server)
        { }

        public override TaskResult Start(TaskExecutionNode node)
        {
            return TaskResult.Success;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            return TaskResult.Success;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            return TaskResult.Success;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var asm = node.RemoteTask as NBehaveAssemblyTask;
            if (asm == null)
                return;
            var assemblies = new[] { asm.AssemblyFile };
            var codeGenListener = new CodeGenEventListener();
            var listener = new NBehaveTaskRunnerListener(node.Children, Server, codeGenListener);

            foreach (var childNode in node.Children)
            {
                var task = childNode.RemoteTask as NBehaveFeatureTask;
                if (task == null)
                    continue;

                Server.TaskStarting(task);
                var evtListener = new MultiOutputEventListener(codeGenListener, listener);
                Server.TaskProgress(task, "Running...");
                var runner = InitializeNBehaveRun(new[] { task.FeatureFile }, assemblies, evtListener);

                var results = runner.Run();
                PublishResults(results, task);
            }
        }

        private void PublishResults(FeatureResults results, NBehaveFeatureTask task)
        {
            Server.TaskOutput(task, "", TaskOutputType.STDOUT);
            var taskResult = GetTaskResult(results);
            string taskResultMessage = "";
            if (taskResult == TaskResult.Skipped)
                taskResultMessage = "Skipped";
            if (taskResult == TaskResult.Inconclusive)
            {
                taskResultMessage = "Pending";
                Server.TaskExplain(task, "See pending step(s) for more information");
            }
            if (taskResult == TaskResult.Error)
            {
                var firstFailure = results.ScenarioResults.First(_ => _.Result is Failed);
                var result = (Failed)firstFailure.Result;
                taskResultMessage = result.Exception.Message;
                var te = new TaskException(result.Exception);
                Server.TaskException(task, new[] { te });
            }
            Server.TaskFinished(task, taskResultMessage, taskResult);
        }

        private TaskResult GetTaskResult(FeatureResults results)
        {
            var taskResult = (results.NumberOfScenariosFound > 0) ? TaskResult.Skipped : TaskResult.Exception;
            taskResult = (results.NumberOfPassingScenarios > 0) ? TaskResult.Success : taskResult;
            taskResult = (results.NumberOfPendingScenarios > 0) ? TaskResult.Inconclusive : taskResult;
            taskResult = (results.NumberOfFailingScenarios > 0) ? TaskResult.Error : taskResult;
            return taskResult;
        }

        private static IRunner InitializeNBehaveRun(IEnumerable<string> featureFiles, IEnumerable<string> assemblies, EventListener evtListener)
        {
            var config = NBehaveConfiguration
                .New
                .SetAssemblies(assemblies)
                .SetEventListener(evtListener)
                .SetScenarioFiles(featureFiles);

            var runner = config.Build();
            return runner;
        }
    }
}