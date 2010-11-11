using System;
using System.ComponentModel.Composition;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof(IStartUpTask))]
    public class LoggingTask : IStartUpTask
    {
        [Export(typeof(IPluginLogger))]
        public IPluginLogger DefaultLogger { get; set; }

        [Import(AllowRecomposition = true)]
        public Lazy<IOutputWindow> OutputWindow { get; set; }

        public void Initialise()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            OutputWindowTarget outputWindowTarget = new OutputWindowTarget(this.OutputWindow);
            outputWindowTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${exception:ToString}";
            config.AddTarget("mail", outputWindowTarget);

            LoggingRule rule1 = new LoggingRule("*", LogLevel.Fatal, outputWindowTarget);
            config.LoggingRules.Add(rule1);
            
            LogManager.Configuration = config;

            DefaultLogger = new PluginLogger(LogManager.GetLogger("default"));
        }
    }

    [Target("OutputWindow")]
    internal sealed class OutputWindowTarget : TargetWithLayoutHeaderAndFooter
    {
        private Lazy<IOutputWindow> _outputWindow;

        public OutputWindowTarget(Lazy<IOutputWindow> outputWindow)
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
            _outputWindow.Value.WriteLine(s);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            this.Output(this.Layout.Render(logEvent));
        }
    }
}
