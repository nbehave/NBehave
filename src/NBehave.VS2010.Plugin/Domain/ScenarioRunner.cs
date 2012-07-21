using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    [Guid("7D6EEAFD-FFC0-4d56-A1A9-256178D1A330")]
    [ComVisible(true)]
    public interface IScenarioRunner
    {
        void Run(bool debug);
        void Run(string documentName, bool debug);
    }

    public class ScenarioRunner : IScenarioRunner
    {
        private readonly IVisualStudioService _visualStudioService;
        private readonly IOutputWindow _outputWindow;
        private readonly IConsoleRunner _consoleRunner;

        public ScenarioRunner(IOutputWindow outputWindow,
            IVisualStudioService visualStudioService,
            IConsoleRunner consoleRunner)
        {
            _outputWindow = outputWindow;
            _visualStudioService = visualStudioService;
            _consoleRunner = consoleRunner;
        }

        public void Run(bool debug)
        {
            var activeDocumentFullName = _visualStudioService.GetActiveDocumentFullName();
            Run(activeDocumentFullName, debug);
        }

        public void Run(string documentName, bool debug)
        {
            _visualStudioService.BuildSolution();
            _outputWindow.Clear();
            _outputWindow.BringToFront();

            RunDocumentFile(documentName, debug);
        }

        private void RunDocumentFile(string documentName, bool debug)
        {
            var pathToNBehaveConsole = _consoleRunner.GetPathToExecutable();
            Run(pathToNBehaveConsole, documentName, debug);
        }

        private void Run(string pathToNBehaveConsole, string documentName, bool debug)
        {
            _visualStudioService.BuildSolution();
            _outputWindow.Clear();

            var assemblyPath = _visualStudioService.GetProjectAssemblyOutputPath();
            // create an xml document, xsl transform it to html and load it in VS
            // or get data back live from the console and show it in visual studio
            var args = FormatArguments(documentName, assemblyPath, debug);

            var process = StartConsole(pathToNBehaveConsole, assemblyPath, args);
            var output = new Task(() =>
            {
                try
                {
                    _outputWindow.BringToFront();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        _outputWindow.WriteLine(process.StandardOutput.ReadLine());
                    }
                }
                catch (Exception exception)
                {
                    _outputWindow.WriteLine(exception.ToString());
                }
            });
            output.Start();

            if (debug)
                _visualStudioService.AttachDebugger(process.Id);
        }

        private string FormatArguments(string documentName, string assemblyPath, bool debug)
        {
            var args = string.Format("\"{0}\" /sf=\"{1}\"", assemblyPath, documentName);

            if (debug)
                args += " /wd";
            return args;
        }

        private Process StartConsole(string pathToNBehaveConsole, string assemblyPath, string args)
        {
            var processStartInfo = new ProcessStartInfo
            {
                Arguments = args,
                CreateNoWindow = true,
                FileName = pathToNBehaveConsole,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(assemblyPath)
            };

            var process = new Process { StartInfo = processStartInfo };
            process.Start();
            return process;
        }
    }
}