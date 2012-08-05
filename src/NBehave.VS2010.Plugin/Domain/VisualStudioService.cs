using System;
using System.IO;
using System.Linq;
using System.Threading;
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

        public string ReferencedAssemblyFolder(string referencedAssembly)
        {
            var project = Project;
            var vsProject = (VSLangProj.VSProject)project.Object;
            var references = vsProject.References.Cast<VSLangProj.Reference>();
            var reference = references.FirstOrDefault(_ => _.Name.Equals(referencedAssembly, StringComparison.InvariantCultureIgnoreCase));
            return (reference == null) ? null : Path.GetDirectoryName(reference.Path);
        }

        public bool Is32Bit
        {
            get
            {
                var platform = Project.ConfigurationManager.ActiveConfiguration.Properties.Item("PlatformTarget").Value.ToString();
                return platform == "x86";
            }
        }

        public string GetProjectAssemblyOutputPath()
        {
            var containingProject = Project;

            var fullPath = containingProject.Properties.Item("FullPath").Value.ToString();
            var outputPath = containingProject.ConfigurationManager
                .ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
            var outputDir = Path.Combine(fullPath, outputPath);
            var outputFileName = containingProject.Properties.Item("OutputFileName").Value.ToString();
            var assemblyPath = Path.Combine(outputDir, outputFileName);

            return assemblyPath;
        }

        public string GetActiveDocumentFullName()
        {
            return _dteService.ActiveDocument.FullName;
        }

        public void AttachDebugger(int processId)
        {
            Process process = _dteService.Debugger.LocalProcesses
                .Cast<Process>()
                .First(proc => proc.ProcessID == processId);
            process.Attach();
        }

        readonly ManualResetEventSlim protectFromCompilingWhenCompiling = new ManualResetEventSlim(false);

        public void BuildSolution()
        {
            if (!protectFromCompilingWhenCompiling.IsSet)
            {
                try
                {
                    protectFromCompilingWhenCompiling.Set();
                    _dteService.Solution.SolutionBuild.Build(true);
                }
                finally
                {
                    protectFromCompilingWhenCompiling.Reset();
                }
            }
            else
            {
                while (protectFromCompilingWhenCompiling.IsSet)
                    System.Threading.Thread.Sleep(50);
            }
        }

        public string GetTargetFrameworkVersion()
        {
            var targetFramework =
                _dteService.ActiveDocument.ProjectItem.ContainingProject.Properties.Item("TargetFrameworkMoniker");
            return (string)targetFramework.Value == ".NETFramework,PackageVersion=v4.0" ? "v4.0" : "V3.5";
        }

        private Project Project
        {
            get
            {
                var project = _dteService.ActiveDocument.ProjectItem.ContainingProject;
                return project;
            }
        }
    }
}