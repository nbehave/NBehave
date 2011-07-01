using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.UI.RichText;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Tiny;
using NBehave.ReSharper.Plugin.UnitTestProvider;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        private static ITinyMessengerHub _hub;
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
            var asm = node.RemoteTask as AssemblyTask;
            if (asm == null)
                return;
            var assemblies = new[] { asm.AssemblyFile };
            using (var listener = new NBehaveTaskRunnerListener(_hub, node.Children, Server))
            {
                foreach (var childNode in node.Children)
                {
                    var task = childNode.RemoteTask as FeatureTask;
                    if (task == null)
                        continue;

                    Server.TaskStarting(task);
                    var codegenWriter = new StringWriter();
                    var codeGenListener = new CodeGenEventListener(codegenWriter);
                    var text = new RichText();
                    var textWriter = new RichTextEventListener(text);
                    var evtListener = new MultiOutputEventListener(codeGenListener, textWriter, listener);
                    Server.TaskProgress(task, "Running...");
                    var runner = InitializeNBehaveRun(new[] {task.FeatureFile}, assemblies, evtListener);

                    var results = runner.Run();
                    var taskResult = GetTaskResult(results);
                    codegenWriter.Flush();
                    PublishResults(results, codegenWriter, task, text, taskResult);
                }
            }
        }

        private void PublishResults(FeatureResults results, StringWriter codegenWriter, FeatureTask task, RichText text, TaskResult taskResult)
        {
            Server.TaskOutput(task, text.Text, TaskOutputType.STDOUT);
            string taskResultMessage = "";
            if (taskResult == TaskResult.Skipped)
                taskResultMessage = "Skipped";
            if (taskResult == TaskResult.Inconclusive)
            {
                taskResultMessage = "Pending";
                var toImplement = codegenWriter.ToString();
                Server.TaskExplain(task, toImplement);
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
            var container = TinyIoCContainer.Current;
            NBehaveInitialiser.Initialise(container, config);
            _hub = container.Resolve<ITinyMessengerHub>();

            return runner;
        }
    }
}