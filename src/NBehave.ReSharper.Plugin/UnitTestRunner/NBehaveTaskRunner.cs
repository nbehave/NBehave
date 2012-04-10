using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NBehave.ReSharper.Plugin.UnitTestProvider;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunner : RecursiveRemoteTaskRunner
    {
        private NBehaveConfiguration config;
        private IFeatureRunner runner;
        private IRunContextEvents context;
        public const string RunnerId = TestProvider.NBehaveId;

        public NBehaveTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

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
            Initialiser.Initialise();
            config = TinyIoCContainer.Current.Resolve<NBehaveConfiguration>();
            runner = TinyIoCContainer.Current.Resolve<IFeatureRunner>();
            context = TinyIoCContainer.Current.Resolve<IRunContextEvents>();
            var asm = node.RemoteTask as NBehaveAssemblyTask;
            if (asm == null)
                return;
            var assemblies = new[] { asm.AssemblyFile };
            var files = node.Children.Select(_ => ((NBehaveFeatureTask)_.RemoteTask).FeatureFile).Distinct().ToList();
            var codeGenListener = new CodeGenEventListener();
            var resharperResultNotifier = new ResharperResultPublisher(node.Children, Server, codeGenListener);
            var listener = new NBehaveTaskRunnerListener(resharperResultNotifier);

            var featureTasks = new List<NBehaveFeatureTask>();
            foreach (var featureNode in node.Children)
            {
                var task = featureNode.RemoteTask as NBehaveFeatureTask;
                featureTasks.Add(task);
                Server.TaskStarting(task);
                Server.TaskProgress(task, "Running...");
            }
            var evtListener = new MultiOutputEventListener(codeGenListener, listener);
            ModifyConfig(files, assemblies, evtListener);
            Run(featureTasks);
        }

        private void Run(IEnumerable<NBehaveFeatureTask> tasks)
        {
            using (var publisher = new ResultPublisher(Server, context))
            {
                runner.Run(config.ScenarioFiles);
                publisher.PublishResults(tasks);
            }
        }

        private void ModifyConfig(IEnumerable<string> featureFiles, IEnumerable<string> assemblies, EventListener evtListener)
        {
            config
                .SetAssemblies(assemblies)
                .SetEventListener(evtListener)
                .SetScenarioFiles(featureFiles);
        }
    }
}