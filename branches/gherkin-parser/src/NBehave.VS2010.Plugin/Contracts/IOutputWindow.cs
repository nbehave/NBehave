using System.Runtime.InteropServices;

namespace NBehave.VS2010.Plugin.Contracts
{
    [Guid("bf2e13f1-c365-4e52-9042-9c5668bd1c23")]
    [ComVisible(true)]
    public interface IOutputWindow
    {
        void WriteLine(string message);
        void BringToFront();
        void Clear();
    }
}