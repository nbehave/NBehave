using System;
using System.IO;
using System.Linq;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    public interface INuGet
    {
        string SolutionPackageVersion(string packageId);
        string ProjectPackageVersion(string packageId);
        string PackageFolderBySolutionPackage(string packageId);
        string PackageFolderByReference(string packageId, string referencedAssembly);
    }

    public class NuGet : INuGet
    {
        private readonly DTE dteService;
        private readonly IVisualStudioService visualStudioService;

        public NuGet(IServiceProvider serviceProvider, IVisualStudioService visualStudioService)
        {
            this.visualStudioService = visualStudioService;
            dteService = serviceProvider.GetService(typeof(SDTE)) as DTE;

            if (dteService == null)
                throw new InvalidOperationException("Was not able to get the EnvDTE service.");
        }

        public string SolutionPackageVersion(string packageId)
        {
            var item = GetSolutionPackageConfigProjectItem();
            var fileName = item.FileNames[1];
            return PackageVersion(packageId, fileName);
        }

        public string PackageFolderBySolutionPackage(string packageId)
        {
            var item = GetSolutionPackageConfigProjectItem();
            var fileName = item.FileNames[1];
            return Path.Combine(Directory.GetParent(Path.GetDirectoryName(fileName)).FullName, "packages");
        }

        public string ProjectPackageVersion(string packageId)
        {
            var item = GetPackagesConfig();
            if (item == null)
                return "";
            return PackageVersion(packageId, item.FileNames[0]);
        }

        public string PackageFolderByReference(string packageId, string referencedAssembly)
        {
            var item = GetPackagesConfig();
            if (item == null)
                return null;
            var ver = PackageVersion(packageId, item.FileNames[0]);
            if (ver == null)
                return null;
            var assemblyFolder = visualStudioService.ReferencedAssemblyFolder(referencedAssembly);
            var packageWithVersion = string.Format("{0}.{1}", packageId, ver).ToLower();
            var idx = assemblyFolder.ToLower().IndexOf(packageWithVersion);
            return assemblyFolder.Substring(0, idx);
        }

        private ProjectItem GetSolutionPackageConfigProjectItem()
        {
            var projs = dteService.Solution.Projects.Cast<Project>().ToList();
            var pNames = projs.FirstOrDefault(_ => _.Name == ".nuget");
            var item = pNames.ProjectItems.Cast<ProjectItem>().FirstOrDefault(_ => _.Name == "packages.config");
            return item;
        }

        private ProjectItem GetPackagesConfig()
        {
            var project = dteService.ActiveDocument.ProjectItem.ContainingProject;
            var projectItems = project.ProjectItems.Cast<ProjectItem>();
            var item = projectItems.FirstOrDefault(_ => _.Name.Equals("packages.config", StringComparison.InvariantCultureIgnoreCase));
            return item;
        }

        private string PackageVersion(string packageId, string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            var node = xml.SelectSingleNode(string.Format(@"packages/package[@id='{0}']", packageId));
            if (node == null)
                return null;
            return node.Attributes["version"].Value;
        }
    }
}