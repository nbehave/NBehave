using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    public class ScenarioRunner
    {
        private readonly IOutputWindow _outputWindow;
        private readonly IVisualStudioService _visualStudioService;

        public ScenarioRunner(IOutputWindow outputWindow, IVisualStudioService visualStudioService)
        {
            _outputWindow = outputWindow;
            _visualStudioService = visualStudioService;
        }

        public void Run(bool debug)
        {
            _visualStudioService.BuildSolution();
            _outputWindow.Clear();

            var assemblyPath = _visualStudioService.GetAssemblyPath();
            var activeDocumentFullName = _visualStudioService.GetActiveDocumentFullName();

            var args = string.Format("\"{0}\" /sf=\"{1}\"", assemblyPath, activeDocumentFullName);

            if (debug)
            {
                args += " /wd";
            }

            var processStartInfo = new ProcessStartInfo
                                       {
                                           Arguments = args,
                                           CreateNoWindow = true,
                                           FileName = GetExecutable(),
                                           RedirectStandardOutput = true,
                                           UseShellExecute = false,
                                           WorkingDirectory = Path.GetDirectoryName(assemblyPath)
                                       };

            var process = new Process
                              {
                                  StartInfo = processStartInfo
                              };


            process.Start();
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
            {
                _visualStudioService.AttachDebugger(process.Id);
            }
        }

        private string GetExecutable()
        {
            var nbehaveRegKey =
                Registry.LocalMachine.OpenSubKey(string.Format("{0}{1}", "SOFTWARE\\NBehave\\",
                                                               typeof (ScenarioRunner).Assembly.GetName().Version));
            var nbehaveConsoleExe = "NBehave-Console.exe";

            if (nbehaveRegKey != null)
            {
                var installDirectory = nbehaveRegKey.GetValue("Install_Dir");


                if (installDirectory != null)
                {
                    var version = _visualStudioService.GetTargetFrameworkVersion();

                    return Path.Combine((string) installDirectory, version, nbehaveConsoleExe);
                }
            }
            return nbehaveConsoleExe;
        }
    }
}