using System.Globalization;
using System.IO;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.NAnt
{
    [TaskName("nbehave")]
    public class NBehaveTask : Task
    {
        private bool dryRun;
        private bool failBuild;
        private string storyOutputPath;
        private FileSet testAssemblies = new FileSet();

        [TaskAttribute("dryRun")]
        public bool DryRun
        {
            get { return dryRun; }
            set { dryRun = value; }
        }

        [TaskAttribute("storyOutputPath")]
        public string StoryOutputPath
        {
            get { return storyOutputPath; }
            set { storyOutputPath = value; }
        }

        [BuildElement("assemblies", Required = true)]
        public FileSet TestAssemblies
        {
            get { return testAssemblies; }
            set { testAssemblies = value; }
        }

        [TaskAttribute("failBuild")]
        public bool FailBuild
        {
            get { return failBuild; }
            set { failBuild = value; }
        }

        protected override void ExecuteTask()
        {
            if (TestAssemblies.FileNames.Count == 0)
                throw new BuildException("At least one test assembly is required");

            LogWriter nantLogWriter = new LogWriter(this, Level.Info, CultureInfo.InvariantCulture);
            PlainTextOutput output = new PlainTextOutput(nantLogWriter);

            WriteHeaderInto(output);

            StoryRunner runner = new StoryRunner();
            runner.IsDryRun = DryRun;

            foreach (string path in testAssemblies.FileNames)
            {
                runner.LoadAssembly(path);
            }

            StoryResults results = runner.Run(CreateEventListenerUsing(nantLogWriter));

            if (dryRun) return;

            WriteResultsInto(output, results);

            if (failBuild)
                FailBuildBasedOn(results);
        }

        private void WriteHeaderInto(PlainTextOutput output)
        {
            output.WriteHeader();
            output.WriteSeparator();
            output.WriteRuntimeEnvironment();
            output.WriteSeparator();
        }

        private void WriteResultsInto(PlainTextOutput output, StoryResults results)
        {
            output.WriteDotResults(results);
            output.WriteSummaryResults(results);
            output.WriteFailures(results);
            output.WritePending(results);
        }

        private void FailBuildBasedOn(StoryResults results)
        {
            if (results.NumberOfFailingScenarios == 0) return;

            StringBuilder exceptionMessage = new StringBuilder();
            foreach (ScenarioResults result in results.ScenarioResults)
            {
                exceptionMessage.AppendLine(result.Message);
                exceptionMessage.AppendLine(result.StackTrace);
                exceptionMessage.AppendLine();
            }

            throw new BuildException(exceptionMessage.ToString());
        }

        private IEventListener CreateEventListenerUsing(TextWriter writer)
        {
            return storyOutputPath.Blank()
                       ? EventListeners.NullEventListener()
                       : new MultiOutputEventListener(EventListeners.FileOutputEventListener(storyOutputPath),
                                                      EventListeners.TextWriterEventListener(writer));
        }

        #region Nested type: EventListeners

        private static class EventListeners
        {
            public static IEventListener NullEventListener()
            {
                return new NullEventListener();
            }

            public static IEventListener FileOutputEventListener(string storyOutputPath)
            {
                return new FileOutputEventListener(storyOutputPath);
            }

            public static IEventListener TextWriterEventListener(TextWriter writer)
            {
                return new TextWriterEventListener(writer);
            }
        }

        #endregion
    }
}
