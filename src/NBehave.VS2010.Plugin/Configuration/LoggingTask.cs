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
        public Logger Logger { get; set; }

        public void Initialise()
        {
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = "${message}";

            // Step 4. Define rules
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            LoggingRule rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            // Example usage
            Logger = LogManager.GetLogger("Example");
            Logger.Trace("trace log message");
            Logger.Debug("debug log message");
            Logger.Info("info log message");
            Logger.Warn("warn log message");
            Logger.Error("error log message");
            Logger.Fatal("fatal log message");

        }
    }
}
