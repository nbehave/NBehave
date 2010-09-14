namespace NBehave.VS2010.Plugin.Contracts
{
    internal interface IVisualStudioService
    {
        string GetAssemblyPath();
        string GetActiveDocumentFullName();
        void AttachDebugger(int id);
        void BuildSolution();
        string GetTargetFrameworkVersion();
    }
}