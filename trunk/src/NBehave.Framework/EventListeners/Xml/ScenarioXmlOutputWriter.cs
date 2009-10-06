using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
	public class ScenarioXmlOutputWriter : XmlOutputBase
	{
		private Timer _currentScenarioExecutionTime;
		private string _currentScenarioTitle;
		private readonly string _belongsToStoryWithTitle;

		public Dictionary<string, ScenarioResult> ScenarioResults { get; private set; }

		public ScenarioXmlOutputWriter(XmlWriter writer, Queue<Action> actions, string belongsToStoryWithTitle)
			: base(writer, actions)
		{
			ScenarioResults = new Dictionary<string, ScenarioResult>();
			_belongsToStoryWithTitle = belongsToStoryWithTitle;
		}

		public void ScenarioCreated(string title)
		{
			_currentScenarioTitle = title;
			_currentScenarioExecutionTime = new Timer();

			var refToScenarioExecutionTime = _currentScenarioExecutionTime;
			refToScenarioExecutionTime.Stop(); //Right now I dont know how to measure the execution time for a scenario /Morgan
			Actions.Enqueue(() =>
			                {
			                	WriteStartElement("scenario", title, refToScenarioExecutionTime);
			                	Writer.WriteAttributeString("outcome", ScenarioResults[title].Result.ToString().ToLower());
			                	while (_messages.Count > 0)
			                	{
			                		_messages.Dequeue().Invoke();
			                	}
			                	Writer.WriteEndElement(); // </scenario>
			                });
		}

		private readonly Queue<Action> _messages = new Queue<Action>();

		public void ScenarioMessageAdded(string message)
		{
			string title = _currentScenarioTitle;
			_messages.Enqueue(() =>
			                  {
			                  	ScenarioMessageAdded(message, title);
			                  });
		}
		
		private void ScenarioMessageAdded(string message, string belongsToScenarioWithTitle)
		{
			Writer.WriteStartElement("actionStep");
			var stepResult = (from r in ScenarioResults[belongsToScenarioWithTitle].ActionStepResults
			                  where message.StartsWith(r.ActionStep, StringComparison.CurrentCulture)
			                  select r).FirstOrDefault();
			
			Writer.WriteAttributeString("name", message);
			if (stepResult != null)
			{
				Writer.WriteAttributeString("outcome", stepResult.Result.ToString().ToLower());
				if (stepResult.Result.GetType()==typeof(Failed))
					Writer.WriteElementString("failure",stepResult.Message);
			}
			Writer.WriteEndElement();
		}

		public override void DoResults(StoryResults results)
		{
			var scenarioResults = ExtractResultsForScenario(results);
			base.DoResults(scenarioResults);
		}

		private StoryResults ExtractResultsForScenario(StoryResults results)
		{
			IEnumerable<ScenarioResult> scenarioResults = from r in results.ScenarioResults
				where r.StoryTitle == _belongsToStoryWithTitle
				&& r.ScenarioTitle == _currentScenarioTitle
				select r;
			var storyResults = new StoryResults();
			foreach (var scenarioResult in scenarioResults)
			{
				ScenarioResults.Add(scenarioResult.ScenarioTitle, scenarioResult);
				storyResults.AddResult(scenarioResult);
			}
			return storyResults;
		}
	}
}