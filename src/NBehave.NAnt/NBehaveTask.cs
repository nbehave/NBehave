using System.Globalization;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NAntCore = NAnt.Core;

namespace NBehave.NAnt
{
    using System.Linq;

    [TaskName("nbehave")]
    public class NBehaveTask : Task
    {
        public NBehaveTask()
        {
            TestAssemblies = new FileSet();
        }

        [TaskAttribute("dryRun")]
        public bool DryRun { get; set; }

        [TaskAttribute("textOutputFile")]
        public string TextOutputFile { get; set; }

        [TaskAttribute("xmlOutputFile")]
        public string XmlOutputFile { get; set; }

        [BuildElement("assemblies", Required = true)]
        public FileSet TestAssemblies { get; set; }

        [BuildElement("scenarioFiles", Required = true)]
        public FileSet ScenarioFiles { get; set; }

        [TaskAttribute("failBuild")]
        public bool FailBuild { get; set; }

        protected override void ExecuteTask()
        {
            if (TestAssemblies.FileNames.Count == 0)
                throw new BuildException("At least one test assembly is required");

            var nantLogWriter = new LogWriter(this, Level.Info, CultureInfo.InvariantCulture);
            var output = new PlainTextOutput(nantLogWriter);

            WriteHeaderInto(output);

            var listener = EventListeners.CreateEventListenerUsing(nantLogWriter,
                                                                              TextOutputFile,
                                                                              XmlOutputFile);

            var runner = NBehaveConfiguration
                .New
                .SetDryRun(DryRun)
                .SetScenarioFiles(ScenarioFiles.FileNames.Cast<string>().ToList())
                .SetAssemblies(TestAssemblies.FileNames.Cast<string>().ToList())
                .SetEventListener(listener)
                .Build();

            var results = runner.Run();

            if (DryRun)
                return;

            if (FailBuild)
                FailBuildBasedOn(results);
        }

        private void WriteHeaderInto(PlainTextOutput output)
        {
            output.WriteHeader();
            output.WriteSeparator();
            output.WriteRuntimeEnvironment();
            output.WriteSeparator();
        }

        private void FailBuildBasedOn(FeatureResults featureResults)
        {
            if (featureResults.NumberOfFailingScenarios == 0) return;

            var exceptionMessage = new StringBuilder();
            foreach (var result in featureResults.SelectMany(_=>_.ScenarioResults))
            {
                exceptionMessage.AppendLine(result.Message);
                exceptionMessage.AppendLine(result.StackTrace);
                exceptionMessage.AppendLine();
            }

            throw new BuildException(exceptionMessage.ToString());
        }
    }
}
