using System;
using System.Runtime.ExceptionServices;
using NBehave.VS2010.Plugin.Contracts;
using NLog;

namespace NBehave.VS2010.Plugin.Domain
{
    public class PluginLogger : IPluginLogger
    {
        private readonly Logger logger;

        public PluginLogger(Logger logger)
        {
            this.logger = logger;
        }

        public void ErrorException(string message, Exception exception)
        {
            logger.ErrorException(message, exception);
        }

        public void FatalException(string message, Exception exception)
        {
            logger.FatalException(message, exception);
        }

        public void LogFirstChanceException(FirstChanceExceptionEventArgs args)
        {
            if (IsExceptionFromNBehave(args.Exception))
                FatalException("", args.Exception);
        }

        private bool IsExceptionFromNBehave(Exception exception)
        {
            return (exception != null && (exception.StackTrace ?? "").ToLower().Contains("nbehave"));
        }
    }
}