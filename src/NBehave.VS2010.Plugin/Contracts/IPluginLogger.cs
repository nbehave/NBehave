using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace NBehave.VS2010.Plugin.Contracts
{
    [Guid("0a91b46f-0311-4bd6-8394-f1d6ad87b978")]
    public interface IPluginLogger
    {
        void ErrorException(string message, Exception exception);
        void FatalException(string message, Exception exception);
        void LogFirstChanceException(FirstChanceExceptionEventArgs args);
    }
}
