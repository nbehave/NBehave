using System;
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

            RunnerBase runner;
            if (string.IsNullOrEmpty(options.file))
                runner = new StoryRunner();
            else
            {
                runner = new ActionStepRunner();
                ((ActionStepRunner)runner).Load(options.file.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
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
            IEventListener listener = CreateEventListener(options);

            StoryResults results = runner.Run(listener);

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
            if (options.HasStoryOutput)
                return new FileOutputEventListener(options.storyOutput);
            if (options.HasStoryXmlOutput)
            {
                var writer = new XmlTextWriter(options.xml, Encoding.UTF8);
                return new XmlOutputEventListener(writer);
            }
            return new NullEventListener();
        }
    }
}