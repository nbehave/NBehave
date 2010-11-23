using System;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class PlainTextOutput : MarshalByRefObject
    {
        private readonly TextWriter _writer;

        public PlainTextOutput(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteLine(string text)
        {
            _writer.WriteLine(text);
        }

        public void WriteHeader()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Version version = executingAssembly.GetName().Version;

            var copyrights = (AssemblyCopyrightAttribute[])
                Attribute.GetCustomAttributes(executingAssembly, typeof(AssemblyCopyrightAttribute));

            _writer.WriteLine("NBehave version {0}", version);

            foreach (AssemblyCopyrightAttribute copyrightAttribute in copyrights)
            {
                _writer.WriteLine(copyrightAttribute.Copyright);
            }

            if (copyrights.Length > 0)
                _writer.WriteLine("All Rights Reserved.");
        }

        public void WriteDotResults(StoryResults results)
        {
            foreach (ScenarioResult result in results.ScenarioResults)
            {
                char resultIndicator;
                if (result.Result.GetType() == typeof(Passed))
                {
                    resultIndicator = '.';
                }
                else if (result.Result.GetType()== typeof(Failed))
                {
                    resultIndicator = 'F';
                }
                else if (result.Result.GetType() == typeof(Pending))
                {
                    resultIndicator = 'P';
                }
                else
                {
                    resultIndicator = '?';
                }
                _writer.Write(resultIndicator);
            }
            _writer.WriteLine();
        }

        public void WriteSummaryResults(StoryResults results)
        {
            _writer.WriteLine("Scenarios run: {0}, Failures: {1}, Pending: {2}", results.NumberOfScenariosFound,
                             results.NumberOfFailingScenarios, results.NumberOfPendingScenarios);
        }

        public void WriteFailures(StoryResults results)
        {
            if (results.NumberOfFailingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Failures:");
                int failureNumber = 1;

                foreach (ScenarioResult result in results.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Failed))
                    {
                        _writer.WriteLine("{0}) {1} ({2}) FAILED", failureNumber, result.StoryTitle,
                                         result.ScenarioTitle);
                        _writer.WriteLine("  {0}", result.Message);
                        _writer.WriteLine("{0}", result.StackTrace);
                        failureNumber++;
                    }
                }
            }
        }

        public void WriteSeparator()
        {
            _writer.WriteLine("");
        }

        public void WritePending(StoryResults results)
        {
            if (results.NumberOfPendingScenarios > 0)
            {
                WriteSeparator();
                _writer.WriteLine("Pending:");
                int pendingNumber = 1;

                foreach (ScenarioResult result in results.ScenarioResults)
                {
                    if (result.Result.GetType() == typeof(Pending))
                    {
                        _writer.WriteLine("{0}) {1} ({2}): {3}", pendingNumber, result.StoryTitle,
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
            _writer.WriteLine(runtimeEnv);
        }
    }
}
