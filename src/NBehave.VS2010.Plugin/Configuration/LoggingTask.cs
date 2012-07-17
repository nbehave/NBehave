using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NBehave.VS2010.Plugin.Configuration
{
    public class LoggingTask : ITinyIocInstaller
    {
        public void Install(TinyIoCContainer container)
        {
            var outputWindow = container.Resolve<IOutputWindow>();
            var config = new LoggingConfiguration();

            var outputWindowTarget = new OutputWindowTarget(outputWindow);
            outputWindowTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${exception:ToString}";
            config.AddTarget("mail", outputWindowTarget);

            var rule1 = new LoggingRule("NBehave.*", LogLevel.Fatal, outputWindowTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;

            var pluginLogger = new PluginLogger(LogManager.GetLogger("default"));
            container.Register<IPluginLogger>(pluginLogger);
        }
    }

    [Target("OutputWindow")]
    internal sealed class OutputWindowTarget : TargetWithLayoutHeaderAndFooter
    {
        private readonly IOutputWindow _outputWindow;

        public OutputWindowTarget(IOutputWindow outputWindow)
        {
            _outputWindow = outputWindow;
        }

        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                Output(Footer.Render(LogEventInfo.CreateNullEvent()));
            }
            base.CloseTarget();
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (Header != null)
            {
                Output(Header.Render(LogEventInfo.CreateNullEvent()));
            }
        }

        private void Output(string s)
        {
            _outputWindow.WriteLine(s);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            Output(Layout.Render(logEvent));
        }
    }
}
