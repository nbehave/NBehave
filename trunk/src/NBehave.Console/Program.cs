using System;
using System.Collections.Generic;
using System.IO;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Console
{
	public class Program
	{
		[STAThread]
		public static int Main(string[] args)
		{
		    var t0 = DateTime.Now;
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
            var runner = new TextRunner(listener);
            runner.Load(options.scenarioFiles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            runner.IsDryRun = options.dryRun;

            foreach (string path in options.Parameters)
            {
                try
                {
                    runner.LoadAssembly(path);
                }
                catch (FileNotFoundException e)
                {
                    output.WriteLine(string.Format("File not found: {0}", path));
                }
            }

            FeatureResults results = runner.Run();
            System.Console.WriteLine("Time Taken {0:0.##}", DateTime.Now.Subtract(t0).TotalSeconds);

            if (options.dryRun)
                return 0;

            int result = results.NumberOfFailingScenarios > 0 ? 2 : 0;
            if(options.pause) {
            	System.Console.WriteLine("Press any key to exit");
            	System.Console.ReadKey();
            }
            
            return result;
        }

        public static IEventListener CreateEventListener(ConsoleOptions options)
        {
            var eventListeners = new List<IEventListener>();
            if (options.HasStoryOutput)
                eventListeners.Add(EventListeners.FileOutputEventListener(options.storyOutput));
            if (options.HasStoryXmlOutput)
                eventListeners.Add(EventListeners.XmlWriterEventListener(options.xml));
            if (eventListeners.Count == 0)
                eventListeners.Add(new ColorfulConsoleOutputEventListener());
            if (options.codegen)
                eventListeners.Add(EventListeners.CodeGenEventListener(System.Console.Out));

            return new MultiOutputEventListener(eventListeners.ToArray());
        }
    }
}