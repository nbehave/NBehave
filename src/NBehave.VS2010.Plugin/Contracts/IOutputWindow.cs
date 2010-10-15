namespace NBehave.VS2010.Plugin.Contracts
{
    public interface IOutputWindow
    {
        void WriteLine(string message);
        void BringToFront();
        void Clear();
    }
}