using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    public interface IConsoleRunner
    {
        string GetPathToExecutable();
    }

    public class ConsoleRunner : IConsoleRunner
    {
        private readonly IVisualStudioService visualStudioService;
        private readonly INuGet nuget;

        public ConsoleRunner(IVisualStudioService visualStudioService, INuGet nuget)
        {
            this.visualStudioService = visualStudioService;
            this.nuget = nuget;
        }

        public string GetPathToExecutable()
        {
            string pathToNBehaveconsole = null;
            if (IsNbehaveRunnersPackageInstalled())
                pathToNBehaveconsole = PathToConsoleExeViaNBehaveRunnersPackageReference();

            if (string.IsNullOrWhiteSpace(pathToNBehaveconsole) && CanFindConsoleExeViaNuGetPackageReference())
                pathToNBehaveconsole = PathToConsoleExeViaNugetPackageReference();

            if (string.IsNullOrWhiteSpace(pathToNBehaveconsole) && CanFindConsoleExeViaRegistry())
                pathToNBehaveconsole = GetExecutableViaRegistry();

            if (string.IsNullOrWhiteSpace(pathToNBehaveconsole))
            {
                //throw nice exception explaining how to fix it
                string msg = string.Format(
                    "Unable to find NBehave-Console.exe{0}" +
                    "To fix this problem you have 2 options{0}" +
                    "1. Install nbehave.runners via nuget in this solution{0}" +
                    "2. Install NBehave using the installer.{0}"
                    , Environment.NewLine);
                throw new FileNotFoundException(msg);
            }

            return pathToNBehaveconsole;
        }

        private string PathToConsoleExeViaNBehaveRunnersPackageReference()
        {
            var folder = nuget.PackageFolderBySolutionPackage("nbehave.runners");
            var exe = LocateConsoleExeInPackageFolder(folder);
            return exe;
        }

        private bool IsNbehaveRunnersPackageInstalled()
        {
            var nbehaveRunners = nuget.SolutionPackageVersion("nbehave.runners");
            return (nbehaveRunners != null);
        }

        private bool CanFindConsoleExeViaNuGetPackageReference()
        {
            var nbehave = nuget.ProjectPackageVersion("nbehave");
            if (nbehave == null)
                return false;
            var nbehave2 = visualStudioService.ReferencedAssemblyFolder("NBehave.Narrator.Framework");
            while (!nbehave2.EndsWith("packages", StringComparison.CurrentCultureIgnoreCase))
                nbehave2 = Directory.GetParent(nbehave2).FullName;
            var exe = LocateConsoleExeInPackageFolder(nbehave2);
            if (exe == null)
                return false;
            return File.Exists(exe);
        }

        private string LocateConsoleExeInPackageFolder(string nbehave2)
        {
            var runner = GetNbehaveRunnersPath(nbehave2);
            if (runner == null)
                return null;
            var exe = GetNBehaveConsoleExePath(runner);
            return exe;
        }

        private string GetNbehaveRunnersPath(string path)
        {
            var dir = new DirectoryInfo(path);
            var runner = dir.GetDirectories("NBehave.Runners.*").FirstOrDefault();
            return runner.FullName;
        }

        private string GetNBehaveConsoleExePath(string path)
        {
            var folder = Path.Combine(Path.Combine(path, "tools", "net40"));
            if (visualStudioService.Is32Bit)
                return Path.Combine(folder, "NBehave-Console-x86.exe");
            return Path.Combine(folder, "NBehave-Console.exe");
        }

        private string PathToConsoleExeViaNugetPackageReference()
        {
            var nbehave = nuget.PackageFolderByReference("nbehave", "NBehave.Narrator.Framework");
            var runner = GetNbehaveRunnersPath(nbehave);
            var exe = GetNBehaveConsoleExePath(runner);
            return exe;
        }

        private bool CanFindConsoleExeViaRegistry()
        {
            return NbehaveRegKey != null;
        }

        private string GetExecutableViaRegistry()
        {
            var nbehaveRegKey = NbehaveRegKey;
            const string nbehaveConsoleExe = "NBehave-Console.exe";

            var installDirectory = nbehaveRegKey.GetValue("Install_Dir");

            if (installDirectory != null)
            {
                var version = visualStudioService.GetTargetFrameworkVersion();

                return Path.Combine((string)installDirectory, version, nbehaveConsoleExe);
            }
            return nbehaveConsoleExe;
        }

        private RegistryKey NbehaveRegKey
        {
            get
            {
                var nbehaveRegKey =
                    Registry.LocalMachine.OpenSubKey(string.Format("{0}{1}", "SOFTWARE\\NBehave\\", GetType().Assembly.GetName().Version));
                return nbehaveRegKey;
            }
        }
    }
}