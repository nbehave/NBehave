using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
	public class PlainTextOutput
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

		public void WriteDotResults(FeatureResults results)
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

		public void WriteSummaryResults(FeatureResults results)
		{
			_writer.WriteLine("Scenarios run: {0}, Failures: {1}, Pending: {2}", results.NumberOfScenariosFound,
			                  results.NumberOfFailingScenarios, results.NumberOfPendingScenarios);
			int actionSteps = CountActionSteps(results);
			int failedSteps = CountFailedActionSteps(results);
			int pendingSteps= CountPendingActionSteps(results);
			_writer.WriteLine("Steps {0}, failed {1}, pending {2}", actionSteps, failedSteps, pendingSteps);
		}
		
		private int CountActionSteps(FeatureResults featureResults)
		{
			return CountPassedActionSteps(featureResults) +
				CountPendingActionSteps(featureResults) +
				CountFailedActionSteps(featureResults);
		}
		
		private int CountPassedActionSteps(FeatureResults featureResults)
		{
			return CountActionStepsOfType(featureResults, typeof(Passed));
		}
		
		private int CountFailedActionSteps(FeatureResults featureResults)
		{
			return CountActionStepsOfType(featureResults, typeof(Failed));
		}
		
		private int CountPendingActionSteps(FeatureResults featureResults)
		{
			return CountActionStepsOfType(featureResults, typeof(Pending));
		}
		private int CountActionStepsOfType(FeatureResults featureResults, Type typeOfStep)
		{
			int sum=0;
			foreach	(var result in featureResults.ScenarioResults)
			{
				var toCount = from r in result.ActionStepResults
					where r.Result.GetType()==typeOfStep
					select r;
				sum += toCount.Count();
			}
			return sum;
		}
		
		public void WriteFailures(FeatureResults results)
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
						_writer.WriteLine("{0}) {1} ({2}) FAILED", failureNumber, result.FeatureTitle,
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

		public void WritePending(FeatureResults results)
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
						_writer.WriteLine("{0}) {1} ({2}): {3}", pendingNumber, result.FeatureTitle,
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
