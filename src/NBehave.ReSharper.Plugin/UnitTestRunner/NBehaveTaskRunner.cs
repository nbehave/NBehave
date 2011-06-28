using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.UI.RichText;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.ReSharper.Plugin.UnitTestProvider;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = TestProvider.NBehaveId;
        private readonly Action<string> _log = s => Console.WriteLine(s);

        public NBehaveTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            _log("NBehaveTaskRunner ctor");
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            _log("NBehaveTaskRunner Start");
            return TaskResult.Success;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            _log("NBehaveTaskRunner Execute");
            return TaskResult.Success;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            _log("NBehaveTaskRunner Finish");
            return TaskResult.Success;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            _log("NBehaveTaskRunner ExecuteRecursive");
            LogTask(node);
            _log("== Children");
            foreach (var child in node.Children)
                LogTask(child);
            _log("==");

            var asm = node.RemoteTask as AssemblyTask;
            if (asm == null)
                return;
            var assemblies = new[] { asm.AssemblyFile };

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
                var evtListener = new MultiOutputEventListener(codeGenListener, textWriter);
                var results = RunNBehave(new[] { task.FeatureFile }, assemblies, evtListener);
                var taskResult = GetTaskResult(results);
                codegenWriter.Flush();
                var toImplement = codegenWriter.ToString();
                Server.TaskOutput(task, text.Text, TaskOutputType.STDOUT);
                Server.TaskOutput(task, toImplement, TaskOutputType.STDOUT);
                if (taskResult == TaskResult.Success)
                    Server.TaskFinished(task, "", taskResult);
                if (taskResult == TaskResult.Skipped)
                    Server.TaskFinished(task, "Skipped", taskResult);
                if (taskResult == TaskResult.Inconclusive)
                    Server.TaskFinished(task, "Pending", taskResult);
                if (taskResult == TaskResult.Error)
                {
                    var firstFailure = results.ScenarioResults.First(_ => _.Result is Failed);
                    Server.TaskFinished(task, firstFailure.Result.Message, taskResult);
                }
            }
        }

        private static TaskResult GetTaskResult(FeatureResults results)
        {
            var taskResult = (results.NumberOfScenariosFound > 0) ? TaskResult.Skipped : TaskResult.Exception;
            taskResult = (results.NumberOfPassingScenarios > 0) ? TaskResult.Success : taskResult;
            taskResult = (results.NumberOfPendingScenarios > 0) ? TaskResult.Inconclusive : taskResult;
            taskResult = (results.NumberOfFailingScenarios > 0) ? TaskResult.Error : taskResult;
            return taskResult;
        }

        private FeatureResults RunNBehave(IEnumerable<string> featureFiles, IEnumerable<string> assemblies, EventListener evtListener)
        {
            _log(string.Format("Running with file: {0}", featureFiles.First()));
            _log(string.Format("Running with assembly: {0}", assemblies.First()));

            var config = NBehaveConfiguration
                .New
                .SetAssemblies(assemblies)
                .SetEventListener(evtListener)
                .SetScenarioFiles(featureFiles);

            var runner = config.Build();
            var results = runner.Run();
            return results;
        }

        //private IEnumerable<string> Assemblies(TaskExecutionNode node)
        //{

        //    //var nodes = new List<TaskExecutionNode> { node };
        //    //nodes.AddRange(node.Children);
        //    //var assemblies = nodes
        //    //    .Where(_ => _.RemoteTask is AssemblyTask)
        //    //    .Select(_ => ((AssemblyTask)_.RemoteTask).AssemblyFile)
        //    //    .Distinct();
        //    //return assemblies;
        //}

        private IEnumerable<string> FeatureFiles(TaskExecutionNode node)
        {
            var nodes = new List<TaskExecutionNode> { node };
            nodes.AddRange(node.Children);
            var featureFiles = nodes
                .Where(_ => _.RemoteTask is FeatureTask)
                .Select(_ => ((FeatureTask)_.RemoteTask).FeatureFile)
                .Distinct();
            return featureFiles;
        }

        private void LogTask(TaskExecutionNode current)
        {
            _log(string.Format("Node: {0}", current.GetType().Name));

            _log(string.Format("Task: {0}", current.RemoteTask.GetType().Name));
            var asmTask = current.RemoteTask as AssemblyLoadTask;
            if (asmTask != null)
            {
                _log(string.Format("AssemblyLoadTask: {0}", asmTask.AssemblyCodeBase));
            }

            var featureTask = current.RemoteTask as FeatureTask;
            if (featureTask != null)
                _log(string.Format("Feature: {0}", featureTask.FeatureFile));

            var assemblyTask = current.RemoteTask as AssemblyTask;
            if (assemblyTask != null)
                _log(string.Format("Assembly: {0}", assemblyTask.AssemblyFile));
        }
    }
}