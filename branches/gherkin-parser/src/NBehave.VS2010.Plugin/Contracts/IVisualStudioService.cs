using System.Runtime.InteropServices;

namespace NBehave.VS2010.Plugin.Contracts
{
    [Guid("733f8cbc-53fc-42af-b722-d5dcf4e3800b")]
    [ComVisible(true)]
    public interface IVisualStudioService
    {
        string GetAssemblyPath();
        string GetActiveDocumentFullName();
        void AttachDebugger(int id);
        void BuildSolution();
        string GetTargetFrameworkVersion();
    }
}