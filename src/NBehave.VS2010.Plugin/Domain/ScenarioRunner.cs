using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
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
        private readonly IVisualStudioService visualStudioService;
        private readonly IOutputWindow outputWindow;
        private readonly IConsoleRunner consoleRunner;
        private XslCompiledTransform xslTransformer;
        private bool htmlOutput = false;

        public ScenarioRunner(IOutputWindow outputWindow,
            IVisualStudioService visualStudioService,
            IConsoleRunner consoleRunner)
        {
            this.outputWindow = outputWindow;
            this.visualStudioService = visualStudioService;
            this.consoleRunner = consoleRunner;
        }

        public void Run(bool debug)
        {
            var activeDocumentFullName = visualStudioService.GetActiveDocumentFullName();
            Run(activeDocumentFullName, debug);
        }

        public void Run(string documentName, bool debug)
        {
            visualStudioService.BuildSolution();
            outputWindow.Clear();
            outputWindow.BringToFront();

            var pathToNBehaveConsole = consoleRunner.GetPathToExecutable();
            Run(pathToNBehaveConsole, documentName, debug);
        }

        private void Run(string pathToNBehaveConsole, string documentName, bool debug)
        {
            var assemblyPath = visualStudioService.GetProjectAssemblyOutputPath();
            string filename = "nbehave_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) + ".xml";
            var xmlOutput = Path.Combine(Path.GetTempPath(), filename);
            var args = FormatArguments(documentName, assemblyPath, xmlOutput, debug);

            var process = StartConsole(pathToNBehaveConsole, assemblyPath, args);
            var output = new Task(() =>
            {
                try
                {
                    outputWindow.BringToFront();

                    while (!process.StandardOutput.EndOfStream)
                        outputWindow.WriteLine(process.StandardOutput.ReadLine());
                    CreateHtmlOutput(xmlOutput);
                }
                catch (Exception exception)
                {
                    outputWindow.WriteLine(exception.ToString());
                }
            });
            output.Start();

            if (debug)
                visualStudioService.AttachDebugger(process.Id);
        }

        private void CreateHtmlOutput(string xmlOutput)
        {
            if (htmlOutput)
            {
                var htmlFile = TransformXmltoHtml(xmlOutput);
                Process.Start(htmlFile);
            }
        }

        private string TransformXmltoHtml(string xmlOutput)
        {
            string outputFilename = xmlOutput + ".html";
            using (XmlWriter output = XmlWriter.Create(outputFilename))
            {
                var input = new XPathDocument(xmlOutput);
                XsltTransformer.Transform(input, output);
            }
            return outputFilename;
        }

        private XslCompiledTransform XsltTransformer
        {
            get
            {
                if (xslTransformer == null)
                {
                    xslTransformer = new XslCompiledTransform();
                    var stream = GetType().Assembly.GetManifestResourceStream(typeof(NBehaveRunnerPackage), "NBehaveResults.xsl");
                    var x = new XmlTextReader(stream);
                    xslTransformer.Load(x);
                }
                return xslTransformer;
            }
        }

        private string FormatArguments(string documentName, string assemblyPath, string xmlOutput, bool debug)
        {
            var args = string.Format("\"{0}\" /sf=\"{1}\" /console", assemblyPath, documentName) +
                string.Format(" /xml=\"{0}\"", xmlOutput);

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