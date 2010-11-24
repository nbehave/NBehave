using System;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    internal class OutputWindow : IOutputWindow
    {
        private readonly IVsOutputWindowPane _outputWindowPane;

        public OutputWindow(IVsOutputWindowPane outputWindowPane)
        {
            _outputWindowPane = outputWindowPane;
        }

        #region IOutputWindow Members

        public void WriteLine(string message)
        {
            _outputWindowPane.OutputString(string.Format("{0}{1}", message, Environment.NewLine));
        }

        public void BringToFront()
        {
            _outputWindowPane.Activate();
        }

        public void Clear()
        {
            _outputWindowPane.Clear();
        }

        #endregion
    }
}