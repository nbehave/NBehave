namespace NBehave.VS2010.Plugin.Contracts
{
    internal interface IOutputWindow
    {
        void WriteLine(string message);
        void BringToFront();
        void Clear();
    }
}