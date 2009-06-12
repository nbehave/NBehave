using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
	public abstract class XmlOutputBase
	{
		private StoryResults _resultsBeforeLastThemeStarted;
		
		protected XmlWriter Writer { get; private set; }
		protected Queue<Action> Actions { get; private set; }

		public int TotalScenarios { get; set; }
		public int TotalScenariosPending { get; set; }
		public int TotalScenariosFailed { get; set; }

		protected XmlOutputBase(XmlWriter writer, Queue<Action> actions)
			: this(writer, actions, new StoryResults())
		{}
		
		protected XmlOutputBase(XmlWriter writer, Queue<Action> actions, StoryResults resultsAlreadyDone)
		{
			Writer = writer;
			Actions = actions;
			CopyScenarioResults(resultsAlreadyDone);
		}
		
		private void CopyScenarioResults(StoryResults resultsAlreadyDone)
		{
			_resultsBeforeLastThemeStarted = new StoryResults();
			foreach(var result in resultsAlreadyDone.ScenarioResults)
				_resultsBeforeLastThemeStarted.AddResult(result);
			
		}

		protected void WriteStartElement(string elementName, string attributeName, Timer result)
		{
			Writer.WriteStartElement(elementName);
			Writer.WriteAttributeString("name", attributeName);
			Writer.WriteAttributeString("time", result.TimeTaken.ToString());
		}

		protected void WriteScenarioResult()
		{
			Writer.WriteAttributeString("scenarios", TotalScenarios.ToString());
			Writer.WriteAttributeString("scenariosFailed", TotalScenariosFailed.ToString());
			Writer.WriteAttributeString("scenariosPending", TotalScenariosPending.ToString());
		}

		protected void UpdateSummary(XmlOutputBase output, StoryResults results)
		{
			var newResults = GetNewStoryResults(results);
			output.TotalScenarios += newResults.NumberOfScenariosFound;
			output.TotalScenariosFailed += (newResults.NumberOfScenariosFound - results.NumberOfPassingScenarios - results.NumberOfPendingScenarios);
			output.TotalScenariosPending += newResults.NumberOfPendingScenarios;
			//_resultsBeforeLastThemeStarted = results;
		}
		
		StoryResults GetNewStoryResults(StoryResults results)
		{
			var newResults = new StoryResults();
			var notHandled = results.ScenarioResults.Except(_resultsBeforeLastThemeStarted.ScenarioResults);
			foreach(var result in notHandled)
				newResults.AddResult(result);
			return newResults;
		}

		public virtual void DoResults(StoryResults results)
		{
			UpdateSummary(this, results);
		}
	}
}
