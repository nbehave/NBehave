using System;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    public class OutputWindow : IOutputWindow
    {
        private readonly IVsOutputWindowPane _outputWindowPane;
        private readonly Dispatcher _dispatcher;

        public OutputWindow(IVsOutputWindowPane outputWindowPane)
        {
            _outputWindowPane = outputWindowPane;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void WriteLine(string message)
        {
            string str = string.Format("{0}{1}", message, Environment.NewLine);
            Action outString = () => _outputWindowPane.OutputStringThreadSafe(str);
            _dispatcher.Invoke(outString);
        }

        public void BringToFront()
        {
            _outputWindowPane.Activate();
        }

        public void Clear()
        {
            _outputWindowPane.Clear();
        }
    }
}