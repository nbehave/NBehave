using System;
using NBehave.VS2010.Plugin.Contracts;
using NLog;

namespace NBehave.VS2010.Plugin.Domain
{
    public class PluginLogger : IPluginLogger
    {
        private Logger _logger;

        public PluginLogger(Logger logger)
        {
            _logger = logger;
        }

        public void ErrorException(string message, Exception exception)
        {
            _logger.ErrorException(message, exception);
        }

        public void FatalException(string message, Exception exception)
        {
            _logger.FatalException(message, exception);
        }
    }
}