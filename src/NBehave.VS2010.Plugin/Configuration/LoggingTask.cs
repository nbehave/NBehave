using System.ComponentModel.Composition;
using NBehave.VS2010.Plugin.Contracts;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof(IStartUpTask))]
    public class LoggingTask : IStartUpTask
    {
        [Export(typeof(Logger))]
        public Logger DefaultLogger { get; set; }

        public void Initialise()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            MailTarget mailTarget = new MailTarget();
            config.AddTarget("mail", mailTarget);

            LoggingRule rule1 = new LoggingRule("*", LogLevel.Fatal, mailTarget);
            config.LoggingRules.Add(rule1);
            
            LogManager.Configuration = config;

            DefaultLogger = LogManager.GetLogger("default");
        }
    }
}
