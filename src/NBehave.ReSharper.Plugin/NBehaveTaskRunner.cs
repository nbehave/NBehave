using System;
using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.ReSharper.Plugin.Task;

namespace NBehave.ReSharper.Plugin
{
    public class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = TestProvider.NBehaveId;

        private Action<string> Log = s => File.AppendAllText(@"C:\Temp\rs.log", s + Environment.NewLine);

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
            return TaskResult.Success;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            Log("NBehaveTaskRunner ExecuteRecursive");
            var assemblyTask = node.RemoteTask as RunScenarioTask;
            if (assemblyTask == null)
                return;
            Log(string.Format("== {0}", assemblyTask.FeatureFile));
            foreach (var current in node.Children)
            {
                var asmTask = current.RemoteTask as RunScenarioTask;
                if (asmTask != null)
                    Log(asmTask.Id);
                //Getfiles in that project
                //var files = Directory.GetFiles(Path.GetDirectoryName(asmTask.AssemblyLocation), "*.feature");
                //add to some list
            }

            //Where is nbehave?
            //string directoryName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            //using (var asmLoader = new AssemblyLoader())
            //{
            //    asmLoader.RegisterAssembly(Assembly.LoadFrom(Path.Combine(directoryName, "NBehave.Narrator.Framework")));
            //}
            throw new ApplicationException("ExecuteRecursive was actually called!!");
        }
    }
}