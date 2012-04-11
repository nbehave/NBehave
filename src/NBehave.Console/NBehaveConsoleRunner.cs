using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Extensions;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Console
{
    /// <summary>
    ///   Console runner for running text based scenarios.
    /// </summary>
    public class NBehaveConsoleRunner
    {
        private class ReturnCode
        {
            public const int InvalidArguments = -1;
            public const int FileNotFound = -2;
            public const int AttachDebuggerTimeout = -3;
        }

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
            var options = CommandLineParser<ConsoleOptions>.Parse(args);

            if (!options.Nologo)
            {
                output.WriteHeader();
                output.WriteSeparator();
                output.WriteRuntimeEnvironment();
                output.WriteSeparator();
            }

            if (options.Help)
            {
                options.ShowHelp();
                return 0;
            }

            if (!options.IsValid())
            {
                System.Console.Error.WriteLine("fatal error: invalid arguments");
                options.ShowHelp();
                return ReturnCode.InvalidArguments;
            }

            if (options.WaitForDebugger)
            {
                WaitForDebuggerToAttach();
                if (!Debugger.IsAttached)
                {
                    output.WriteLine("fatal error: timeout while waiting for debugger to attach");
                    return ReturnCode.AttachDebuggerTimeout;
                }
            }

            var assemblies = options.Parameters.ToArray().Select(assembly => assembly).Cast<string>().ToList();
            var config = NBehaveConfiguration.New
                .SetScenarioFiles((options.ScenarioFiles).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .SetDryRun(options.DryRun)
                .SetAssemblies(assemblies)
                .UseTagsFilter(options.Tags.Select(_=>_.Split(',')))
                .SetEventListener(CreateEventListener(options));

            FeatureResults featureResults = null;

            try
            {
                featureResults = Run(config);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                System.Console.WriteLine(string.Format("File not found: {0}", fileNotFoundException.FileName));
                return ReturnCode.FileNotFound;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }

            PrintTimeTaken(t0);

            if (options.DryRun)
            {
                return 0;
            }

            if (options.Pause)
            {
                System.Console.WriteLine("Press any key to exit");
                System.Console.ReadKey();
            }
            return (featureResults == null) ? -1 : featureResults.NumberOfFailingScenarios;
        }

        private static FeatureResults Run(NBehaveConfiguration config)
        {
            IRunner runner = config.Build();
            FeatureResults featureResults = runner.Run();
            return featureResults;
        }

        private static void WaitForDebuggerToAttach()
        {
            var countdown = 15000;
            const int waitTime = 200;

            while (!Debugger.IsAttached && countdown >= 0)
            {
                Thread.Sleep(waitTime);
                countdown -= waitTime;
            }
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

        public static EventListener CreateEventListener(ConsoleOptions options)
        {
            var eventListeners = new List<EventListener>();
            if (options.HasStoryOutput)
                eventListeners.Add(EventListeners.FileOutputEventListener(options.StoryOutput));

            if (options.HasStoryXmlOutput)
                eventListeners.Add(EventListeners.XmlWriterEventListener(options.Xml));

            if (options.Console)
                eventListeners.Add(EventListeners.ColorfulConsoleOutputEventListener());

            if (eventListeners.Count == 0)
                eventListeners.Add(EventListeners.ColorfulConsoleOutputEventListener());

            if (options.Codegen)
                eventListeners.Add(EventListeners.CodeGenEventListener(System.Console.Out));

            return new MultiOutputEventListener(eventListeners.ToArray());
        }
    }
}