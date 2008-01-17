using System;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    [Obsolete("Use NBehave.Narrator.Framework.PlainTextOutput passing in Console.Out instead")]
    public class ConsoleOutput
    {
        public void WriteHeader()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Version version = executingAssembly.GetName().Version;

            AssemblyCopyrightAttribute[] copyrights =
                (AssemblyCopyrightAttribute[])
                Attribute.GetCustomAttributes(executingAssembly, typeof (AssemblyCopyrightAttribute));

            System.Console.WriteLine("NBehave version {0}", version);

            foreach (AssemblyCopyrightAttribute copyrightAttribute in copyrights)
            {
                System.Console.WriteLine(copyrightAttribute.Copyright);
            }

            if (copyrights.Length > 0)
                System.Console.WriteLine("All Rights Reserved.");
        }

        public void WriteDotResults(StoryResults results)
        {
            foreach (ScenarioResults result in results.ScenarioResults)
            {
                char resultIndicator;
                switch(result.ScenarioResult)
                {
                    case ScenarioResult.Passed:
                        resultIndicator = '.';
                        break;
                    case ScenarioResult.Failed:
                        resultIndicator = 'F';
                        break;
                    case ScenarioResult.Pending:
                        resultIndicator = 'P';
                        break;
                    default:
                        resultIndicator = '?';
                        break;
                }
                System.Console.Write(resultIndicator);
            }
            System.Console.WriteLine();
        }

        public void WriteSummaryResults(StoryResults results)
        {
            System.Console.WriteLine("Scenarios run: {0}, Failures: {1}, Pending: {2}", results.NumberOfScenariosFound, results.NumberOfFailingScenarios, results.NumberOfPendingScenarios);
        }

        public void WriteFailures(StoryResults results)
        {
            if (results.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                System.Console.WriteLine("Failures:");
                int failureNumber = 1;

                foreach (ScenarioResults result in results.ScenarioResults)
                {
                    if (result.ScenarioResult == ScenarioResult.Failed)
                    {
                        System.Console.WriteLine("{0}) {1} ({2}) FAILED", failureNumber, result.StoryTitle,
                                                 result.ScenarioTitle);
                        System.Console.WriteLine("  {0}", result.Message);
                        System.Console.WriteLine("{0}", result.StackTrace);
                        failureNumber++;
                    }
                }
            }
        }

        public void WriteSeparator()
        {
            System.Console.WriteLine("");
        }

        public void WritePending(StoryResults results)
        {
            if (results.NumberOfPendingScenarios > 0)
            {
                WriteSeparator();
                System.Console.WriteLine("Pending:");
                int pendingNumber = 1;

                foreach (ScenarioResults result in results.ScenarioResults)
                {
                    if (result.ScenarioResult == ScenarioResult.Pending)
                    {
                        System.Console.WriteLine("{0}) {1} ({2}): {3}", pendingNumber, result.StoryTitle,
                                                 result.ScenarioTitle, result.Message);
                        pendingNumber++;
                    }
                }
            }
        }

        public void WriteRuntimeEnvironment()
        {
            string runtimeEnv =
                string.Format("Runtime Environment -\r\n   OS Version: {0}\r\n  CLR Version: {1}", Environment.OSVersion,
                              Environment.Version);
            System.Console.WriteLine(runtimeEnv);
        }
    }
}
