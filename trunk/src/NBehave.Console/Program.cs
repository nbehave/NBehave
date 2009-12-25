using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;

namespace NBehave.Console
{
	public class Program
	{
		[STAThread]
		public static int Main(string[] args)
		{
			var output = new PlainTextOutput(System.Console.Out);
			var options = new ConsoleOptions(args);

			if (!options.nologo)
			{
				output.WriteHeader();
				output.WriteSeparator();
				output.WriteRuntimeEnvironment();
				output.WriteSeparator();
			}

			if (options.help)
			{
				options.Help();
				return 0;
			}

			if (!options.Validate())
			{
				System.Console.Error.WriteLine("fatal error: invalid arguments");
				options.Help();
				return 2;
			}

            IEventListener listener = CreateEventListener(options);
            RunnerBase runner;
			if (string.IsNullOrEmpty(options.scenarioFiles))
				runner = new StoryRunner(listener);
			else
			{
				runner = new TextRunner(listener);
				((TextRunner)runner).Load(options.scenarioFiles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
			}
			runner.IsDryRun = options.dryRun;

			foreach (string path in options.Parameters)
			{
				try
				{
					runner.LoadAssembly(path);
				}
				catch (FileNotFoundException)
				{
					output.WriteLine(string.Format("File not found: {0}", path));
				}
			}
			
			StoryResults results = runner.Run();

			if (options.dryRun)
				return 0;

			output.WriteDotResults(results);
			output.WriteSummaryResults(results);
			output.WriteFailures(results);
			output.WritePending(results);

			int result = results.NumberOfFailingScenarios > 0 ? 2 : 0;

			return result;
		}

		public static IEventListener CreateEventListener(ConsoleOptions options)
		{
			var eventListeners = new List<IEventListener>();
			if (options.HasStoryOutput)
				eventListeners.Add(new FileOutputEventListener(options.storyOutput));
			if (options.HasStoryXmlOutput)
				eventListeners.Add(EventListeners.XmlWriterEventListener(options.xml));
			if (eventListeners.Count == 0)
				eventListeners.Add(new ColorfulOutputEventListener());

			return new MultiOutputEventListener(eventListeners.ToArray());
		}
	}
	
	public class ColorfulOutputEventListener : IEventListener
	{
		public void StoryCreated(string story)
		{
			System.Console.WriteLine(story);
		}
		
		public void StoryMessageAdded(string message)
		{
		}
		
		public void ScenarioCreated(string scenarioTitle)
		{
			System.Console.WriteLine("Scenario: " + scenarioTitle);
		}
		
		public void ScenarioMessageAdded(string message)
		{
			var color = System.ConsoleColor.Green;
			if (message.EndsWith("FAILED"))
				color = System.ConsoleColor.Red;
			if (message.EndsWith("PENDING"))
				color = System.ConsoleColor.Gray;
			
			WriteColorString(message, color);
		}
		
		public void RunStarted()
		{}
		
		public void RunFinished()
		{
			System.Console.WriteLine("");
		}
		
		public void ThemeStarted(string name)
		{
			WriteColorString(name, System.ConsoleColor.Yellow);
		}
		
		public void ThemeFinished()
		{
		}
		
		public void StoryResults(StoryResults results)
		{}

		private void WriteColorString(string text, System.ConsoleColor color)
		{
			var currentColor = System.Console.ForegroundColor;
			System.Console.ForegroundColor=color;
			System.Console.WriteLine(text);
			System.Console.ForegroundColor=currentColor;
		}
	}
}