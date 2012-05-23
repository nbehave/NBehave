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

        private StringStep Step(string step)
        {
            return new StringStep(step, "");
        }

        private Result Passed { get { return new Passed(); } }

        protected virtual void EstablishContext()
        {
            var memStream = new MemoryStream();
            var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

            var story = new Feature("StoryTitle");
            var scenarioResult = new ScenarioResult(story, "ScenarioTitle");
            var actionStepResult = new StepResult(Step("Given Foo"), Passed);
            scenarioResult.AddActionStepResult(actionStepResult);

            var eventsReceived = new List<EventReceived>
									 {
										 new EventReceived("", EventType.RunStart),
										 new EventReceived("StoryTitle", EventType.FeatureStart),
										 new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
										 new EventReceived("ScenarioTitle", EventType.ScenarioStart),
										 new ScenarioResultEventReceived(scenarioResult),
										 new EventReceived("", EventType.FeatureFinished),
										 new EventReceived("", EventType.RunFinished)
									 };

            _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
        }

        [TestFixture]
        public class StepNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/step";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, new List<EventReceived>());
                var result = new StepResult(Step("Given Foo"), Passed);
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
                var actionStepResult1 = new StepResult(Step("Given a"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult(Step("When b"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult(Step("Then c"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("ScenarioTitle", EventType.ScenarioStart),
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
        public class BackgroundNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/background";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var feature = new Feature("FeatureTitle");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var bgStepResult1 = new BackgroundStepResult("background title", new StepResult(Step("Given a"), Passed));
                scenarioResult.AddActionStepResult(bgStepResult1);
                var bgStepResult2 = new BackgroundStepResult("background title", new StepResult(Step("And b"), Passed));
                scenarioResult.AddActionStepResult(bgStepResult2);
                var actionStepResult1 = new StepResult(Step("Given a"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult(Step("When b"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult(Step("Then c"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("ScenarioTitle", EventType.ScenarioStart),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };

                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.DoBackground(scenarioResult);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void Should_have_background_node()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node, Is.Not.Null);
            }

            [Test]
            public void NodeShouldHaveNameAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["name"].Value, Is.EqualTo("background title"));
            }

            [Test]
            public void NodeShouldHaveOutcomeAttribute()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["outcome"].Value, Is.EqualTo("passed"));
            }
        }

        [TestFixture]
        public class FeatureNode : XmlOutputWriterSpec
        {
            const string XPathToNode = @"/feature";

            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var feature = new Feature("FeatureTitle");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult(Step("Given a"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult(Step("When b"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult(Step("Then c"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived("FeatureTitle", EventType.FeatureStart),
				                             new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
				                             new EventReceived("ScenarioTitle", EventType.ScenarioStart),
				                             new ScenarioResultEventReceived(scenarioResult)
				                         };
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.DoFeature(eventsReceived[0]);
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void ShouldHaveFeatureNode()
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
        public class WhenMultipleStoriesHaveScenariosWithSameTitle : XmlOutputWriterSpec
        // Then WTF?
        {
            private const string XPathToNode = @"//results";
            protected override void EstablishContext()
            {
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);

                var eventsReceived = new List<EventReceived>
				                        {
				                            new EventReceived("", EventType.RunStart),
				                        };
                eventsReceived.AddRange(CreateFirstFeature());
                eventsReceived.AddRange(CreateSecondFeature());
                eventsReceived.Add(new EventReceived("", EventType.RunFinished));
                _xmlOutputWriter = new XmlOutputWriter(xmlWriter, eventsReceived);
                _xmlOutputWriter.WriteAllXml();
                xmlWriter.Flush();
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                _xmlDoc.Load(memStream);
            }

            IEnumerable<EventReceived> CreateFirstFeature()
            {
                var feature = new Feature("First feature");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult(Step("Given a"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult(Step("When b"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult(Step("Then c"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
				                         {
				                             new EventReceived(feature.Title, EventType.FeatureStart),
				                             new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
				                             new EventReceived("ScenarioTitle", EventType.ScenarioStart),
				                             new ScenarioResultEventReceived(scenarioResult),
                                             new EventReceived(feature.Title, EventType.FeatureFinished)
				                         };
                return eventsReceived;
            }

            IEnumerable<EventReceived> CreateSecondFeature()
            {
                var feature = new Feature("Second feature");
                var scenarioResult = new ScenarioResult(feature, "ScenarioTitle");
                var actionStepResult1 = new StepResult(Step("Given a"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult1);
                var actionStepResult2 = new StepResult(Step("When b"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult2);
                var actionStepResult3 = new StepResult(Step("Then c"), Passed);
                scenarioResult.AddActionStepResult(actionStepResult3);

                var eventsReceived = new List<EventReceived>
										 {
											 new EventReceived(feature.Title, EventType.FeatureStart),
											 new EventReceived("As a x\nI want y\nSo That z", EventType.FeatureNarrative),
											 new EventReceived("ScenarioTitle", EventType.ScenarioStart),
											 new ScenarioResultEventReceived(scenarioResult),
                                             new EventReceived(feature.Title, EventType.FeatureFinished)
										 };
                return eventsReceived;
            }

            [Test]
            public void ResultsNodeShouldHaveFeatureCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                var storyNode = node.Attributes["features"];
                Assert.That(storyNode, Is.Not.Null);
                Assert.That(storyNode.Value, Is.EqualTo("2"));
            }

            [Test]
            public void ResultsNodeShouldHaveScenarioCount()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode);
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("2"));
                Assert.That(node.Attributes["scenariosFailed"].Value, Is.EqualTo("0"));
                Assert.That(node.Attributes["scenariosPending"].Value, Is.EqualTo("0"));
            }

            [Test]
            public void FirstFeatureShouldHaveOneScenario()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/features/feature[@name='First feature']");
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
            }

            [Test]
            public void SecondFeatureShouldHaveOneScenario()
            {
                var node = _xmlDoc.SelectSingleNode(XPathToNode + @"/features/feature[@name='Second feature']");
                Assert.That(node.Attributes["scenarios"].Value, Is.EqualTo("1"));
            }
        }
    }
}
