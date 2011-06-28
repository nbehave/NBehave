using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Console
{
    /// <summary>
    ///   Console runner for running text based scenarios.
    /// </summary>
    public class NBehaveConsoleRunner
    {
        /// <summary>
        ///   NBehave-Console [inputfiles] [options]
        ///   Runs a set of NBehave stories from the console
        ///   You may specify one or more assemblies.    
        ///   Options:            
        ///   Options that take values may use an equal sign, a colon
        ///   or a space to separate the option from its value.
        /// </summary>
        /// <param name = "args">
        ///   Arguments for the runner, use /? for help.
        /// </param>
        /// <returns>
        ///   Returns zero for success, two for error.
        /// </returns>
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

            if (options.waitForDebugger)
            {
                var countdown = 15000;
                const int waitTime = 200;

                while (!Debugger.IsAttached && countdown >= 0)
                {
                    Thread.Sleep(waitTime);
                    countdown -= waitTime;
                }

                if (!Debugger.IsAttached)
                {
                    output.WriteLine("fatal error: timeout while waiting for debugger to attach");
                    return 2;
                }
            }

            var assemblies = options.Parameters.ToArray().Select(assembly => assembly).Cast<string>().ToList();
            var config = NBehaveConfiguration.New
                .SetScenarioFiles(options.scenarioFiles.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
                .SetDryRun(options.dryRun)
                .SetAssemblies(assemblies)
                .SetEventListener(CreateEventListener(options));

            var runner = new TextRunner(config);

            FeatureResults results;

            try
            {
                results = config.Build().Run();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                System.Console.WriteLine(string.Format("File not found: {0}", fileNotFoundException.FileName));
                return 2;
            }

            PrintTimeTaken(t0);

            if (options.dryRun)
            {
                return 0;
            }

            var result = results.NumberOfFailingScenarios > 0 ? 2 : 0;
            if (options.pause)
            {
                System.Console.WriteLine("Press any key to exit");
                System.Console.ReadKey();
            }

            return result;
        }

        private static void PrintTimeTaken(DateTime t0)
        {
            var timeTaken = DateTime.Now.Subtract(t0).TotalSeconds;
            if (timeTaken >= 60)
            {
                int totalMinutes = Convert.ToInt32(Math.Floor(timeTaken / 60));
                int seconds = Convert.ToInt32(timeTaken - totalMinutes * 60.0);
                System.Console.WriteLine("Time Taken {0}m {1:0.#}s", totalMinutes, seconds);
            }
            else
                System.Console.WriteLine("Time Taken {0:0.#}s", timeTaken);
        }

        internal static EventListener CreateEventListener(ConsoleOptions options)
        {
            var eventListeners = new List<EventListener>();
            if (options.HasStoryOutput)
                eventListeners.Add(EventListeners.FileOutputEventListener(options.storyOutput));

            if (options.HasStoryXmlOutput)
                eventListeners.Add(EventListeners.XmlWriterEventListener(options.xml));

            if (options.console)
                eventListeners.Add(new ColorfulConsoleOutputEventListener());

            if (eventListeners.Count == 0)
                eventListeners.Add(new ColorfulConsoleOutputEventListener());

            if (options.codegen)
                eventListeners.Add(EventListeners.CodeGenEventListener(System.Console.Out));

            return new MultiOutputEventListener(eventListeners.ToArray());
        }
    }
}