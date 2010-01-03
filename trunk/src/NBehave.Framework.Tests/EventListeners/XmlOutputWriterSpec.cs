using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;
using NUnit.Framework;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NBehave.Narrator.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
	[Context]
	public abstract class XmlOutputWriterSpec
	{
	    private XmlOutputWriter _xmlOutputWriter;
	    private XmlDocument _xmlDoc;
		
		[SetUp]
		public void SetUp()
		{
			Establish_context();
		}
		
		protected virtual void Establish_context()
		{
			var memStream = new MemoryStream();
			var _xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

			var story = new Feature("StoryTitle");
			var scenarioResult = new ScenarioResult(story, "ScenarioTitle");
			var actionStepResult = new ActionStepResult("Given Foo", new Passed());
			scenarioResult.AddActionStepResult(actionStepResult);

			var eventsReceived = new List<EventReceived>();
			eventsReceived.Add(new EventReceived("", EventType.RunStart));
			eventsReceived.Add(new EventReceived("", EventType.ThemeStarted));
			eventsReceived.Add(new EventReceived("StoryTitle", EventType.FeatureCreated));
			eventsReceived.Add(new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative));
			eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
			eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));
			eventsReceived.Add(new EventReceived("", EventType.ThemeFinished));
			eventsReceived.Add(new EventReceived("", EventType.RunFinished));

			_xmlOutputWriter = new XmlOutputWriter(_xmlWriter, eventsReceived);
		}
		
		[Context]
		public class ActionStep_node : XmlOutputWriterSpec
		{
			const string _xPathToNode = @"/actionStep";
			
			protected override void Establish_context()
			{
				var memStream = new MemoryStream();
				var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
				_xmlOutputWriter = new XmlOutputWriter(xmlWriter, new List<EventReceived>());
				var result = new ActionStepResult("Given Foo" , new Passed());
				_xmlOutputWriter.DoActionStep(result);
				xmlWriter.Flush();
				_xmlDoc = new XmlDocument();
				memStream.Seek(0,SeekOrigin.Begin);
				_xmlDoc.Load(memStream);
			}
			
			[Specification]
			public void Should_have_actionStep_node()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node, Is.Not.Null);
			}

			[Specification]
			public void Node_should_have_name_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["name"].Value, Is.EqualTo("Given Foo"));
			}
			
			[Specification]
			public void Node_should_have_outcome()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["outcome"].Value, Is.EqualTo("passed"));
			}
		}

		[Context]
		public class Scenario_node : XmlOutputWriterSpec
		{
			const string _xPathToNode = @"/scenario";
			
			protected override void Establish_context()
			{
				var memStream = new MemoryStream();
				var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

				var feature = new Feature("FeatureTitle");
				var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
				var actionStepResult1 = new ActionStepResult("Given a", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult1);
				var actionStepResult2 = new ActionStepResult("When b", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult2);
				var actionStepResult3 = new ActionStepResult("Then c", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult3);

				var eventsReceived = new List<EventReceived>();
				eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
				eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));

				_xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
				_xmlOutputWriter.DoScenario(eventsReceived[0], scenarioResult);
				xmlWriter.Flush();
				_xmlDoc = new XmlDocument();
				memStream.Seek(0,SeekOrigin.Begin);
				_xmlDoc.Load(memStream);
			}
			
			[Specification]
			public void Should_have_scenario_node()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node, Is.Not.Null);
			}

			[Specification]
			public void Node_should_have_name_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["name"].Value, Is.EqualTo("ScenarioTitle"));
			}
			
			[Specification]
			public void Node_should_have_time_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["time"], Is.Not.Null);
			}
			
			[Specification]
			public void Node_should_have_outcome()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["outcome"].Value, Is.EqualTo("passed"));
			}
		}

		[Context]
		public class Story_node : XmlOutputWriterSpec
		{
			const string _xPathToNode = @"/story";
			
			protected override void Establish_context()
			{
				var memStream = new MemoryStream();
				var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

				var feature = new Feature("FeatureTitle");
				var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
				var actionStepResult1 = new ActionStepResult("Given a", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult1);
				var actionStepResult2 = new ActionStepResult("When b", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult2);
				var actionStepResult3 = new ActionStepResult("Then c", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult3);

				var eventsReceived = new List<EventReceived>();
                eventsReceived.Add(new EventReceived("FeatureTitle", EventType.FeatureCreated));
				eventsReceived.Add(new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative));
				eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
                eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));
				_xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
				_xmlOutputWriter.DoStory("StoryTitle", eventsReceived[0]);
				xmlWriter.Flush();
				_xmlDoc = new XmlDocument();
				memStream.Seek(0,SeekOrigin.Begin);
				_xmlDoc.Load(memStream);
			}
			
			[Specification]
			public void Should_have_story_node()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node, Is.Not.Null);
			}

			[Specification]
			public void Node_should_have_name_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo("FeatureTitle"));
			}
			
			[Specification]
			public void Node_should_have_time_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["time"], Is.Not.Null);
			}
			
			[Specification]
			public void Node_should_have_scenarios_subnode()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode + @"/scenarios");
				Assert.That(node, Is.Not.Null);
			}

			[Specification]
			public void Node_should_have_scenario_count_attributes()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
				Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
				Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
			}
		}
		
		[Context]
		public class Theme_node : XmlOutputWriterSpec
		{
			const string _xPathToNode = @"/theme";
			
			protected override void Establish_context()
			{
				var memStream = new MemoryStream();
				var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

				var feature = new Feature("FeatureTitle");
				var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
				var actionStepResult1 = new ActionStepResult("Given a", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult1);
				var actionStepResult2 = new ActionStepResult("When b", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult2);
				var actionStepResult3 = new ActionStepResult("Then c", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult3);

				var eventsReceived = new List<EventReceived>();
				eventsReceived.Add(new EventReceived("", EventType.ThemeStarted));
                eventsReceived.Add(new EventReceived("FeatureTitle", EventType.FeatureCreated));
				eventsReceived.Add(new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative));
				eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
                eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));
				_xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
				eventsReceived.Add(new EventReceived("", EventType.ThemeFinished));
				_xmlOutputWriter.DoTheme(eventsReceived[0]);
				xmlWriter.Flush();
				_xmlDoc = new XmlDocument();
				memStream.Seek(0,SeekOrigin.Begin);
				_xmlDoc.Load(memStream);
			}
			
			[Specification]
			public void Should_have_theme_node()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node, Is.Not.Null);
			}

			[Specification]
			public void Node_should_have_name_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["name"].Value, Is.EqualTo(""));
			}
			
			[Specification]
			public void Node_should_have_time_attribute()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["time"], Is.Not.Null);
			}
			
			[Specification]
			public void Node_should_have_story_count()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				var storyNode = node.Attributes["stories"];
				Assert.That(storyNode, Is.Not.Null);
				Assert.That(storyNode.Value, Is.EqualTo("1"));
			}
			
			[Specification]
			public void Node_should_have_scenario_count()
			{
				var node = _xmlDoc.SelectSingleNode(_xPathToNode);
				Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
				Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
				Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
			}
		}

		[Context]
		public class When_multiple_stories_have_Scenarios_with_same_title : XmlOutputWriterSpec
			// Then WTF?
		{
			private const string XPathToNode = @"//theme";
			protected override void Establish_context()
			{
				var memStream = new MemoryStream();
				var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

				var eventsReceived =new List<EventReceived>();
				eventsReceived.Add(new EventReceived("", EventType.RunStart));
				eventsReceived.Add(new EventReceived("", EventType.ThemeStarted));
				eventsReceived.AddRange(CreateFirstStory());
				eventsReceived.AddRange(CreateSecondStory());
				eventsReceived.Add(new EventReceived("",EventType.ThemeFinished));
				eventsReceived.Add(new EventReceived("", EventType.RunFinished));
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.WriteAllXml();
				xmlWriter.Flush();
				_xmlDoc = new XmlDocument();
				memStream.Seek(0,SeekOrigin.Begin);
				_xmlDoc.Load(memStream);
			}

			List<EventReceived> CreateFirstStory()
			{
				var feature = new Feature("First feature");
				var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
				var actionStepResult1 = new ActionStepResult("Given a", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult1);
				var actionStepResult2 = new ActionStepResult("When b", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult2);
				var actionStepResult3 = new ActionStepResult("Then c", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult3);

				var eventsReceived = new List<EventReceived>();
				eventsReceived.Add(new EventReceived(feature.Title, EventType.FeatureCreated));
				eventsReceived.Add(new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative));
				eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
                eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));
				return eventsReceived;
			}

			List<EventReceived> CreateSecondStory()
			{
				var feature = new Feature("Second story");
				var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
				var actionStepResult1 = new ActionStepResult("Given a", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult1);
				var actionStepResult2 = new ActionStepResult("When b", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult2);
				var actionStepResult3 = new ActionStepResult("Then c", new Passed());
				scenarioResult.AddActionStepResult(actionStepResult3);

				var eventsReceived = new List<EventReceived>();
				eventsReceived.Add(new EventReceived(feature.Title, EventType.FeatureCreated));
				eventsReceived.Add(new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative));
				eventsReceived.Add(new EventReceived("ScenarioTitle", EventType.ScenarioCreated));
                eventsReceived.Add(new ScenarioResultEventReceived(scenarioResult));
				return eventsReceived;
			}
			
			[Specification]
			public void Theme_node_should_have_story_count()
			{
				var node = _xmlDoc.SelectSingleNode(XPathToNode);
				var storyNode = node.Attributes["stories"];
				Assert.That(storyNode, Is.Not.Null);
				Assert.That(storyNode.Value, Is.EqualTo("2"));
			}
			
			[Specification]
			public void Theme_node_should_have_scenario_count()
			{
				var node = _xmlDoc.SelectSingleNode(XPathToNode);
				Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("2"));
				Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
				Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
			}
			
			[Specification]
			public void First_story_should_have_one_scenario()
			{
				var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/stories/story[@name='First feature']");
				Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
			}
			
			[Specification]
			public void Second_story_should_have_one_scenario()
			{
				var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/stories/story[@name='Second story']");
				Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
			}
		}
	}
}
