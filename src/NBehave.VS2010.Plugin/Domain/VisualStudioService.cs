using System;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    internal class VisualStudioService : IVisualStudioService
    {
        private readonly DTE _dteService;

        public VisualStudioService(IServiceProvider serviceProvider)
        {
            _dteService = serviceProvider.GetService(typeof(SDTE)) as DTE;
            
            if (_dteService == null)
                throw new InvalidOperationException("Was not able to get the EnvDTE service.");
        }

        public string GetAssemblyPath()
        {
            var containingProject = _dteService.ActiveDocument.ProjectItem.ContainingProject;

            var fullPath = containingProject.Properties.Item("FullPath").Value.ToString();
            var outputPath =
                containingProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
            var outputDir = Path.Combine(fullPath, outputPath);
            var outputFileName = containingProject.Properties.Item("OutputFileName").Value.ToString();
            var assemblyPath = Path.Combine(outputDir, outputFileName);

            return assemblyPath;
        }

        public string GetActiveDocumentFullName()
        {
            return _dteService.ActiveDocument.FullName;
        }

        public void AttachDebugger(int id)
        {
            _dteService.Debugger.LocalProcesses
                .Cast<Process>()
                .Where(proc => proc.ProcessID == id)
                .First()
                .Attach();
        }

        public void BuildSolution()
        {
            _dteService.Solution.SolutionBuild.Build(true);
        }

        public string GetTargetFrameworkVersion()
        {
            var targetFramework =
                _dteService.ActiveDocument.ProjectItem.ContainingProject.Properties.Item("TargetFrameworkMoniker");
            return (string) targetFramework.Value == ".NETFramework,Version=v4.0" ? "v4.0" : "V3.5";
        }
    }
}