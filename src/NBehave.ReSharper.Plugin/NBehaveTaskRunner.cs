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

            //var assemblyTask = node.RemoteTask as FeatureTask;
            //if (assemblyTask == null)
            //{
            //    _finish = TaskResult.Error;
            //    return;
            //}
            foreach (var current in node.Children)
            {
                LogTask(current);

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
            //throw new ApplicationException("ExecuteRecursive was actually called!!");
        }

        private void LogTask(TaskExecutionNode current)
        {
            Log(string.Format("Node: {0}", current.GetType().Name));
            
            Log(string.Format("Task: {0}", current.RemoteTask.GetType().Name));
            var asmTask = current.RemoteTask as AssemblyLoadTask;
            if (asmTask != null)
                Log("AssemblyLoadTask");

            var featureTask = current.RemoteTask as FeatureTask;
            if (featureTask != null)
                Log(string.Format("Feature: {0}", featureTask.FeatureFile));

            var assemblyTask = current.RemoteTask as AssemblyTask;
            if (assemblyTask != null)
                Log(string.Format("Assembly: {0}", assemblyTask.AssemblyFile));
        }
    }
}