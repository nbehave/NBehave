using System;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using System.IO;
using System.Xml;
using System.Text;


namespace NBehave.Console
{
    public class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            PlainTextOutput output = new PlainTextOutput(System.Console.Out);
            ConsoleOptions options = new ConsoleOptions(args);

            if (!options.nologo)
            {
                output.WriteHeader();
                output.WriteSeparator();
                output.WriteRuntimeEnvironment();
                output.WriteSeparator();
            }

            int result = 0;

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


            StoryRunner runner = new StoryRunner();
            runner.IsDryRun = options.dryRun;

            foreach (string path in options.Parameters)
            {
                try
                {
                    runner.LoadAssembly(path);
                }
                catch (FileNotFoundException e)
                {
                    output.WriteLine(string.Format("File not found: {0}", e.FileName));
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

            result = results.NumberOfFailingScenarios > 0 ? 2 : 0;

            return result;
        }

        public static IEventListener CreateEventListener(ConsoleOptions options)
        {
            if (options.HasStoryOutput)
                return new NBehave.Narrator.Framework.EventListeners.FileOutputEventListener(options.storyOutput);
            if (options.HasStoryXmlOutput)
            {
                XmlTextWriter writer = new XmlTextWriter(options.xml, Encoding.UTF8);
                return new NBehave.Narrator.Framework.EventListeners.Xml.XmlOutputEventListener(writer);
            }
            return new NullEventListener();
        }
    }
}
