using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
	[Context]
	public class ScenarioXmlOutputWriterSpec
	{
		private const string ActionStepText = "Given foo";
		private const string ScenarioTitle = "the scenario";
		private XmlDocument _xmlDoc;
		
		[SetUp]
		public void Establish_context()
		{
			var sw = new MemoryStream();
			XmlWriter xmlWriter = new XmlTextWriter(sw, Encoding.UTF8);
			Queue<Action> actionQueue = new Queue<Action>();
			Story story= new Story("a story");
			ScenarioXmlOutputWriter scenarioXmlWriter =
				new ScenarioXmlOutputWriter(xmlWriter, actionQueue, story.Title);
			
			scenarioXmlWriter.ScenarioCreated(ScenarioTitle);
			scenarioXmlWriter.ScenarioMessageAdded(ActionStepText);

			StoryResults storyResults = CreateResult(story);
			scenarioXmlWriter.DoResults(storyResults);
			while(actionQueue.Count>0)
				actionQueue.Dequeue().Invoke();
			xmlWriter.Flush();
			sw.Seek(0, SeekOrigin.Begin);
			//var s = (new StreamReader(sw)).ReadToEnd();
			_xmlDoc = new XmlDocument();
			_xmlDoc.Load(sw);
		}

		StoryResults CreateResult(Story story)
		{
			ScenarioResult scenarioResult = new ScenarioResult(story, ScenarioTitle);
			ActionStepResult actionStepResult = new ActionStepResult(ActionStepText,
			                                                         new ActionStepResult(ActionStepText, new Passed()));
			scenarioResult.AddActionStepResult(actionStepResult);
			StoryResults storyResults = new StoryResults();
			storyResults.AddResult(scenarioResult);
			return storyResults;
		}

		[Specification]
		public void Should_have_one_scenario_node()
		{
			var nodes = _xmlDoc.SelectNodes("//scenario");
			Assert.AreEqual(1, nodes.Count);
		}

		[Specification]
		public void Should_have_outcome_attribute_on_scenario_node()
		{
			var node = _xmlDoc.SelectSingleNode("//scenario");
			Assert.AreEqual("passed", node.Attributes["outcome"].Value);
		}
	
		[Specification]
		public void Should_have_one_actionStep_node()
		{
			var nodes = _xmlDoc.SelectNodes("//scenario/actionStep");
			Assert.AreEqual(1, nodes.Count);
		}

		[Specification]
		public void Should_have_outcome_attribute_on_actionStep_node()
		{
			var node = _xmlDoc.SelectSingleNode("//scenario/actionStep");
			Assert.AreEqual("passed", node.Attributes["outcome"].Value);
		}}
}
