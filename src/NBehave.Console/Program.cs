using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Text;
using System.Reflection;

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
                int countdown = 15000;
                int waitTime = 200;

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

            var m = new Program();
            FeatureResults results = m.SetupAndRunStories(options, output);
            m.PrintTimeTaken(t0);

            if (options.dryRun)
                return 0;

            int result = results.NumberOfFailingScenarios > 0 ? 2 : 0;
            if (options.pause)
            {
                System.Console.WriteLine("Press any key to exit");
                System.Console.ReadKey();
            }

            return result;
        }

        string _currentAssemblyPath = ".";
        private FeatureResults SetupAndRunStories(ConsoleOptions options, PlainTextOutput output)
        {
            IEventListener listener = CreateEventListener(options);

            var d = AppDomain.CurrentDomain;
            d.AssemblyResolve += AsmResolve;
            var runner = RunnerFactory.CreateTextRunner(options.Parameters.Cast<string>(), listener);

            runner.Load(options.scenarioFiles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            runner.IsDryRun = options.dryRun;
            foreach (string path in options.Parameters)
            {
                try
                {
                    var p = Path.GetDirectoryName(Path.GetFullPath(path));
                    _currentAssemblyPath = p;
                    runner.LoadAssembly(path);
                }
                catch (FileNotFoundException)
                {
                    output.WriteLine(string.Format("File not found: {0}", path));
                }
            }

            return runner.Run(output);
        }

        private Assembly AsmResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(new char[] { ',' })[0].Trim() + ".dll";
            var f = Path.Combine(_currentAssemblyPath, name);
            return Assembly.LoadFrom(f);
        }

        private static string GetPath(Assembly assembly)
        {
            string directory = Path.GetDirectoryName((new System.Uri(assembly.CodeBase)).LocalPath);
            var path = Path.Combine(directory, "languages.yml");
            return path;
        }

        private void PrintTimeTaken(DateTime t0)
        {
            double timeTaken = DateTime.Now.Subtract(t0).TotalSeconds;
            if (timeTaken >= 60)
            {
                int totalMinutes = Convert.ToInt32(Math.Floor(timeTaken / 60));
                int seconds = Convert.ToInt32(timeTaken - totalMinutes * 60.0);
                System.Console.WriteLine("Time Taken {0}m {1:0.#}s", totalMinutes, seconds);
            }
            else
                System.Console.WriteLine("Time Taken {0:0.#}s", timeTaken);
        }

        public IEventListener CreateEventListener(ConsoleOptions options)
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