namespace NBehave.VS2010.Plugin.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using NBehave.VS2010.Plugin.Contracts;
    using NBehave.VS2010.Plugin.Domain;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class LoggingTask : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var outputWindow = container.Resolve<IOutputWindow>();
            var config = new LoggingConfiguration();

            var outputWindowTarget = new OutputWindowTarget(outputWindow);
            outputWindowTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${exception:ToString}";
            config.AddTarget("mail", outputWindowTarget);

            var rule1 = new LoggingRule("*", LogLevel.Fatal, outputWindowTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;

            var pluginLogger = new PluginLogger(LogManager.GetLogger("default"));

            container.Register(Component.For<IPluginLogger>().Instance(pluginLogger));
        }
    }

    [Target("OutputWindow")]
    internal sealed class OutputWindowTarget : TargetWithLayoutHeaderAndFooter
    {
        private IOutputWindow _outputWindow;

        public OutputWindowTarget(IOutputWindow outputWindow)
        {
            this._outputWindow = outputWindow;
        }

        // Methods
        protected override void CloseTarget()
        {
            if (base.Footer != null)
            {
                this.Output(base.Footer.Render(LogEventInfo.CreateNullEvent()));
            }
            base.CloseTarget();
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (base.Header != null)
            {
                this.Output(base.Header.Render(LogEventInfo.CreateNullEvent()));
            }
        }

        private void Output(string s)
        {
            _outputWindow.WriteLine(s);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            this.Output(this.Layout.Render(logEvent));
        }
    }
}
