using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NBehave.Console.Remoting;
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

            StoryResults results = SetupAndRunStories(options, output);

            if (options.dryRun)
                return 0;

            output.WriteDotResults(results);
            output.WriteSummaryResults(results);
            output.WriteFailures(results);
            output.WritePending(results);

            int result = results.NumberOfFailingScenarios > 0 ? 2 : 0;

            return result;
        }

        private static StoryResults SetupAndRunStories(ConsoleOptions options, PlainTextOutput output)
        {
            IEventListener listener = CreateEventListener(options);
            IEventListener remotableListener = new DelegatingListener(listener);

            string assemblyWithConfigFile = null;
            foreach (string path in options.Parameters)
            {
                if(File.Exists(path + ".config"))
                {
                    assemblyWithConfigFile = path + ".config";
                    break;
                }
            }

            RemotableStoryRunner runner;
            if (assemblyWithConfigFile != null)
            {
                var configFileInfo = new FileInfo(assemblyWithConfigFile);
                AppDomainSetup ads = new AppDomainSetup
                                         {
                                             ConfigurationFile = configFileInfo.Name,
                                             ApplicationBase = configFileInfo.DirectoryName //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                         };
                AppDomain ad = AppDomain.CreateDomain("NBehave story runner", null, ads);
                
                var bootstrapper = (AppDomainBootstrapper) ad.CreateInstanceFromAndUnwrap(typeof(AppDomainBootstrapper).Assembly.Location, typeof(AppDomainBootstrapper).FullName);
                bootstrapper.InitializeDomain(new[] { Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configFileInfo.DirectoryName });

                runner = (RemotableStoryRunner)ad.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(RemotableStoryRunner).FullName);
            }
            else
            {
                runner = new RemotableStoryRunner();
            }
            return runner.SetupAndRunStories(remotableListener, options.scenarioFiles, options.Parameters, options.dryRun);
        }

        public static IEventListener CreateEventListener(ConsoleOptions options)
        {
            var eventListeners = new List<IEventListener>();
            if (options.HasStoryOutput)
                eventListeners.Add(new FileOutputEventListener(options.storyOutput));
            if (options.HasStoryXmlOutput)
                eventListeners.Add(EventListeners.XmlWriterEventListener(options.xml));
            if (eventListeners.Count == 0)
                eventListeners.Add(new NullEventListener());

            return new MultiOutputEventListener(eventListeners.ToArray());
        }
    }
}