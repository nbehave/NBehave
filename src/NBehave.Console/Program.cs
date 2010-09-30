using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

            if (options.waitForDebugger)
            {
                var countdown = 5000;
                var waitTime = 200;

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

            var listener = CreateEventListener(options);
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

            var results = runner.Run();
            PrintTimeTaken(t0);

            if (options.dryRun)
                return 0;

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
                var totalMinutes = Convert.ToInt32(Math.Floor(timeTaken / 60));
                System.Console.WriteLine("Time Taken {0}m {1:0.#}s", totalMinutes, timeTaken - 60);
            }
            else
                System.Console.WriteLine("Time Taken {0:0.#}s", timeTaken);
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