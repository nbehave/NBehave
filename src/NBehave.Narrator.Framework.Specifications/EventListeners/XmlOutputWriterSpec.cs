using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public abstract class XmlOutputWriterSpec
    {
        private XmlOutputWriter _xmlOutputWriter;
        private XmlDocument _xmlDoc;

        [SetUp]
        public void SetUp()
        {
            EstablishContext();
        }

        protected virtual void EstablishContext()
        {
            var memStream = new MemoryStream();
            var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

            var story = new Feature("StoryTitle");
            var scenarioResult = new ScenarioResult(story, "ScenarioTitle");
            var actionStepResult = new StepResult("Given Foo", new Passed());
            scenarioResult.AddActionStepResult(actionStepResult);

            var eventsReceived = new List<EventReceived>
									 {
										 new EventReceived("", EventType.RunStart),
										 new EventReceived("", EventType.ThemeStarted),
										 new EventReceived("StoryTitle", EventType.FeatureCreated),
										 new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
										 new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
										 new ScenarioResultEventReceived(scenarioResult),
										 new EventReceived("", EventType.ThemeFinished),
										 new EventReceived("", EventType.RunFinished)
									 };

            _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
        }

        [TestFixture]
        public class ActionStepNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/actionStep";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, new List<EventReceived>());
                var result = new StepResult("Given Foo", new Passed());
                _xmlOutputWriter.DoActionStep(result);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void ShouldHaveActionStepNode()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveNameAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo("Given Foo"));
            }

            [Test]
            public void NodeShouldHaveOutcome()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["outcome"].Value, Is.EqualTo("passed"));
            }
        }

        [TestFixture]
        public class ScenarioNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/scenario";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var feature = new Feature("FeatureTitle");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult("Given a", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult("When b", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult("Then c", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };

                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.DoScenario(eventsReceived[0], scenarioResult);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void ShouldHaveScenarioNode()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveNameAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo("ScenarioTitle"));
            }

            [Test]
            public void NodeShouldHaveTimeAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["time"], Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveOutcome()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["outcome"].Value, Is.EqualTo("passed"));
            }
        }

        [TestFixture]
        public class StoryNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/story";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var feature = new Feature("FeatureTitle");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult("Given a", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult("When b", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult("Then c", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("FeatureTitle", EventType.FeatureCreated),
				                             new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
				                             new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.DoStory("StoryTitle", eventsReceived[0]);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void ShouldHaveStoryNode()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveNameAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo("FeatureTitle"));
            }

            [Test]
            public void NodeShouldHaveTimeAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["time"], Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveScenariosSubnode()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/scenarios");
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveScenarioCountAttributes()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
                Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
                Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
            }
        }

        [TestFixture]
        public class ThemeNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/theme";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var feature = new Feature("FeatureTitle");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult("Given a", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult("When b", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult("Then c", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("", EventType.ThemeStarted),
				                             new EventReceived("FeatureTitle", EventType.FeatureCreated),
				                             new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
				                             new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                eventsReceived.Add(new EventReceived("", EventType.ThemeFinished));
                _xmlOutputWriter.DoTheme(eventsReceived[0]);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void ShouldHaveThemeNode()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveNameAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo(""));
            }

            [Test]
            public void NodeShouldHaveTimeAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["time"], Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveStoryCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                var storyNode = node.Attributes["stories"];
                Assert.That(storyNode, Is.Not.Null);
                Assert.That(storyNode.Value, Is.EqualTo("1"));
            }

            [Test]
            public void NodeShouldHaveScenarioCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
                Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
                Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
            }
        }

        [TestFixture]
        public class WhenMultipleStoriesHaveScenariosWithSameTitle : XmlOutputWriterSpec
        // Then WTF?
        {
            private const string XPathToNode = @"//theme";
            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var eventsReceived = new List<EventReceived>
				                        {
				                            new EventReceived("", EventType.RunStart),
				                            new EventReceived("", EventType.ThemeStarted)
				                        };
                eventsReceived.AddRange(CreateFirstStory());
                eventsReceived.AddRange(CreateSecondStory());
                eventsReceived.Add(new EventReceived("", EventType.ThemeFinished));
                eventsReceived.Add(new EventReceived("", EventType.RunFinished));
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.WriteAllXml();
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            IEnumerable<EventReceived> CreateFirstStory()
            {
                var feature = new Feature("First feature");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult("Given a", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult("When b", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult("Then c", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived(feature.Title, EventType.FeatureCreated),
				                             new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
				                             new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };
                return eventsReceived;
            }

            IEnumerable<EventReceived> CreateSecondStory()
            {
                var feature = new Feature("Second story");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult("Given a", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult("When b", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult("Then c", new Passed());
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
										 {
											 new EventReceived(feature.Title, EventType.FeatureCreated),
											 new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
											 new EventReceived("ScenarioTitle", EventType.ScenarioCreated),
											 new ScenarioResultEventReceived(scenarioResult)
										 };
                return eventsReceived;
            }

            [Test]
            public void ThemeNodeShouldHaveStoryCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                var storyNode = node.Attributes["stories"];
                Assert.That(storyNode, Is.Not.Null);
                Assert.That(storyNode.Value, Is.EqualTo("2"));
            }

            [Test]
            public void ThemeNodeShouldHaveScenarioCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("2"));
                Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
                Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
            }

            [Test]
            public void FirstStoryShouldHaveOneScenario()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/stories/story[@name='First feature']");
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
            }

            [Test]
            public void SecondStoryShouldHaveOneScenario()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/stories/story[@name='Second story']");
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
            }
        }
    }
}
