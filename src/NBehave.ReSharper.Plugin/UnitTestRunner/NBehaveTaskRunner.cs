using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly Action<string> Log = s => File.AppendAllText(@"C:\Temp\rs.log", s + Environment.NewLine);
        private TaskResult _finish;

        public NBehaveTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            Log("NBehaveTaskRunner ctor");
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            Log("NBehaveTaskRunner Start");
            return TaskResult.Success;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            Log("NBehaveTaskRunner Execute");
            return TaskResult.Success;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            Log("NBehaveTaskRunner Finish");
            return _finish;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            _finish = TaskResult.Success;
            Log("NBehaveTaskRunner ExecuteRecursive");
            LogTask(node);

            var assemblies = Assemblies(node);
            var featureFiles = FeatureFiles(node);

            TextWriter writer = new StringWriter();
            EventListener codeGenListener = new CodeGenEventListener(writer);
            var evtListener = new MultiOutputEventListener(codeGenListener, EventListeners.ColorfulConsoleOutputEventListener());
            var config = NBehaveConfiguration
                .New
                .SetAssemblies(assemblies)
                .SetEventListener(evtListener)
                .SetScenarioFiles(featureFiles);

            var runner = config.Build();
            runner.Run();

            writer.Flush();
            var toImplement = writer.ToString();
            Console.WriteLine(toImplement);
        }

        private static IEnumerable<string> Assemblies(TaskExecutionNode node)
        {
            var assemblies = node.Children
                .Where(_ => _.RemoteTask is AssemblyTask)
                .Select(_ => ((AssemblyTask) _.RemoteTask).AssemblyFile)
                .Distinct();
            return assemblies;
        }

        private static IEnumerable<string> FeatureFiles(TaskExecutionNode node)
        {
            var featureFiles = node.Children
                .Where(_ => _.RemoteTask is FeatureTask)
                .Select(_ => ((FeatureTask) _.RemoteTask).FeatureFile)
                .Distinct();
            return featureFiles;
        }

        private void LogTask(TaskExecutionNode current)
        {
            Log(string.Format("Node: {0}", current.GetType().Name));

            Log(string.Format("Task: {0}", current.RemoteTask.GetType().Name));
            var asmTask = current.RemoteTask as AssemblyLoadTask;
            if (asmTask != null)
            {
                Log(string.Format("AssemblyLoadTask: {0}", asmTask.AssemblyCodeBase));
            }

            var featureTask = current.RemoteTask as FeatureTask;
            if (featureTask != null)
                Log(string.Format("Feature: {0}", featureTask.FeatureFile));

            var assemblyTask = current.RemoteTask as AssemblyTask;
            if (assemblyTask != null)
                Log(string.Format("Assembly: {0}", assemblyTask.AssemblyFile));
        }
    }
}