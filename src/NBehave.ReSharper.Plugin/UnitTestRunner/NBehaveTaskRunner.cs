using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Configuration;
using NBehave.EventListeners;
using NBehave.EventListeners.CodeGeneration;
using NBehave.Internal;

using NBehave.ReSharper.Plugin.UnitTestProvider;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public partial class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        private NBehaveConfiguration config;
        public const string RunnerId = TestProvider.NBehaveId;

        public NBehaveTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var asm = node.RemoteTask as NBehaveAssemblyTask;
            if (asm == null)
                return;
            Initialize(node);

            var featureTasks = GetFeatureTasks(node);
            NotifyTasksStarting(featureTasks.ToList());
            var runner = new TextRunner(config);
            runner.Run();
        }

        private IEnumerable<NBehaveFeatureTask> GetFeatureTasks(TaskExecutionNode node)
        {
            var featureTasks = node.Children.Select(_ => (NBehaveFeatureTask)_.RemoteTask);
            return featureTasks;
        }

        private void NotifyTasksStarting(IEnumerable<NBehaveFeatureTask> featureTasks)
        {
            foreach (var task in featureTasks)
            {
                Server.TaskStarting(task);
                //Server.TaskProgress(task, "Running...");
            }
        }

        private void Initialize(TaskExecutionNode node)
        {
            config = NBehaveConfiguration.New;

            var codeGenListener = new CodeGenEventListener();
            var resharperResultNotifier = new ResharperResultPublisher(node.Children, Server, codeGenListener);
            var listener = new NBehaveTaskRunnerListener(resharperResultNotifier);

            var files = node.Children.Select(_ => ((NBehaveFeatureTask)_.RemoteTask).FeatureFile).Distinct().ToList();
            var asm = (NBehaveAssemblyTask)node.RemoteTask;
            var assemblies = new[] { asm.AssemblyFile };
            var eventListener = new MultiOutputEventListener(codeGenListener, listener);
            ModifyConfig(files, assemblies, eventListener);
        }

        private void ModifyConfig(IEnumerable<string> featureFiles, IEnumerable<string> assemblies, EventListener eventListener)
        {
            config
                .SetAssemblies(assemblies)
                .SetEventListener(eventListener)
                .SetScenarioFiles(featureFiles);
        }
    }
}