using System;
using System.IO;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class PlainTextOutput
    {
        private readonly TextWriter writer;

        public PlainTextOutput(TextWriter writer)
        {
            this.writer = writer;
        }

        public void WriteLine(string text)
        {
            writer.WriteLine(text);
        }

        public void WriteHeader()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Version version = executingAssembly.GetName().Version;

            AssemblyCopyrightAttribute[] copyrights =
                (AssemblyCopyrightAttribute[])
                Attribute.GetCustomAttributes(executingAssembly, typeof(AssemblyCopyrightAttribute));

            writer.WriteLine("NBehave version {0}", version);

            foreach (AssemblyCopyrightAttribute copyrightAttribute in copyrights)
            {
                writer.WriteLine(copyrightAttribute.Copyright);
            }

            if (copyrights.Length > 0)
                writer.WriteLine("All Rights Reserved.");
        }

        public void WriteDotResults(StoryResults results)
        {
            foreach (ScenarioResults result in results.ScenarioResults)
            {
                char resultIndicator;
                switch (result.ScenarioResult)
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
                writer.Write(resultIndicator);
            }
            writer.WriteLine();
        }

        public void WriteSummaryResults(StoryResults results)
        {
            writer.WriteLine("Scenarios run: {0}, Failures: {1}, Pending: {2}", results.NumberOfScenariosFound,
                             results.NumberOfFailingScenarios, results.NumberOfPendingScenarios);
        }

        public void WriteFailures(StoryResults results)
        {
            if (results.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                writer.WriteLine("Failures:");
                int failureNumber = 1;

                foreach (ScenarioResults result in results.ScenarioResults)
                {
                    if (result.ScenarioResult == ScenarioResult.Failed)
                    {
                        writer.WriteLine("{0}) {1} ({2}) FAILED", failureNumber, result.StoryTitle,
                                         result.ScenarioTitle);
                        writer.WriteLine("  {0}", result.Message);
                        writer.WriteLine("{0}", result.StackTrace);
                        failureNumber++;
                    }
                }
            }
        }

        public void WriteSeparator()
        {
            writer.WriteLine("");
        }

        public void WritePending(StoryResults results)
        {
            if (results.NumberOfPendingScenarios > 0)
            {
                WriteSeparator();
                writer.WriteLine("Pending:");
                int pendingNumber = 1;

                foreach (ScenarioResults result in results.ScenarioResults)
                {
                    if (result.ScenarioResult == ScenarioResult.Pending)
                    {
                        writer.WriteLine("{0}) {1} ({2}): {3}", pendingNumber, result.StoryTitle,
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
            writer.WriteLine(runtimeEnv);
        }
    }
}
