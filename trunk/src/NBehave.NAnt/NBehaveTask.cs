using System.Collections.Generic;
using System.Globalization;
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

		[BuildElement("scenarioFiles", Required = false)]
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

			RunnerBase runner;
			if (ScenarioFiles == null || ScenarioFiles.FileNames.Count == 0)
				runner = new StoryRunner { IsDryRun = DryRun };
			else
			{
				runner = new TextRunner();
				((TextRunner)runner).Load(GetFileNames(ScenarioFiles));
			}

			foreach (string path in TestAssemblies.FileNames)
			{
				runner.LoadAssembly(path);
			}

			StoryResults results = runner.Run(EventListeners.CreateEventListenerUsing(nantLogWriter, 
			                                                                          TextOutputFile,
			                                                                          XmlOutputFile));

			if (DryRun) return;

			WriteResultsInto(output, results);

			if (FailBuild)
				FailBuildBasedOn(results);
		}

		private IEnumerable<string> GetFileNames(FileSet files)
		{
			foreach (var fileName in files.FileNames)
			{
				yield return fileName;
			}
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

			var exceptionMessage = new StringBuilder();
			foreach (ScenarioResult result in results.ScenarioResults)
			{
				exceptionMessage.AppendLine(result.Message);
				exceptionMessage.AppendLine(result.StackTrace);
				exceptionMessage.AppendLine();
			}

			throw new BuildException(exceptionMessage.ToString());
		}
	}
}
