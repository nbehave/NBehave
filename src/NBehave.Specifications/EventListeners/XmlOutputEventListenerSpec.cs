using System;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.EventListeners.Xml;
using NBehave.Extensions;
using NBehave.Hooks;
using NBehave.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Specifications.EventListeners
{
    [ActionSteps]
    public class XmlOutputEventListenerSpec
    {
        private XmlDocument _xmlDoc;
        private string[] _feature;

        [OneTimeSetUp]
        public virtual void Setup()
        {
            var memStream = new MemoryStream();
            var listener = new XmlOutputEventListener(new XmlTextWriter(memStream, Encoding.UTF8));

            ConfigurationNoAppDomain
                .New
                .SetScenarioFiles(_feature)
                .SetAssemblies(new[] {GetType().Assembly.Location})
                .SetEventListener(listener)
                .Build()
                .Run();

            _xmlDoc = new XmlDocument();
            memStream.Seek(0, 0);
            _xmlDoc.Load(memStream);
        }

        [TestFixture]
        public class WhenRunningWithXmlListener : XmlOutputEventListenerSpec
        {
            public override void Setup()
            {
                _feature = new[] {TestFeatures.FeaturesAndScenarios};
                base.Setup();
            }

            [Test]
            public void ShouldHaveXmlInTheSpecifiedXmlWriter()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results"));
            }

            [Test]
            public void ResultsNodeShouldHaveDateAndTimeAttributes()
            {
                Assert.AreEqual(DateTime.Today.ToShortDateString(),
                                _xmlDoc.SelectSingleNode("results").Attributes["date"].Value);
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["time"].Value);
            }

            [Test]
            public void ResultsNodeHouldHaveNameAttribute()
            {
                Assert.IsTrue(_xmlDoc.SelectSingleNode("results").Attributes["name"].Value.Trim().StartsWith("NBehave"));
            }

            [Test]
            public void ResultsNodeShouldHaveVersionAttribute()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["version"].Value);
            }

            [Test]
            public void ShouldHaveScenariosInResultsNode()
            {
                var outcome = _xmlDoc.SelectSingleNode("results").Attributes["scenarios"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(6));
            }

            [Test]
            public void ResultsNodeShouldContainAttributeAboutExecutionTime()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["executionTime"].Value);
            }

            [Test]
            public void ResultsNodeShouldHaveOnePendingScenario()
            {
                var pending = _xmlDoc.SelectSingleNode(@"results").Attributes["scenariosPending"].Value;
                Assert.AreEqual("1", pending);
            }

            [Test]
            public void ResultsNodeShouldHaveOneFailedScenarios()
            {
                Assert.AreEqual("1",
                                _xmlDoc.SelectSingleNode(@"results").Attributes["scenariosFailed"].
                                    Value);
            }

            [Test]
            public void FeatureNodeShouldHaveANameAttribute()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("//feature[@name='S1']"));
            }

            [Test]
            public void FeatureShouldHaveANarrativeChildElement()
            {
                Assert.IsNotNull(
                    _xmlDoc.SelectSingleNode("results/features/feature[@name='S1']/narrative"));
                var narrative = _xmlDoc.SelectSingleNode("results/features/feature/narrative").InnerText;
                StringAssert.Contains("As a X1\r\n", narrative);
                StringAssert.Contains("I want Y1\r\n", narrative);
                StringAssert.Contains("So that Z1" + Environment.NewLine, narrative);
            }

            [Test]
            public void FeatureNodeShouldHaveSummary()
            {
                Assert.AreEqual("3",
                                _xmlDoc.SelectSingleNode(@"results/features/feature").Attributes[
                                    "scenarios"].Value);
                Assert.AreEqual("0",
                                _xmlDoc.SelectSingleNode(@"results/features/feature").Attributes[
                                    "scenariosFailed"].Value);
                Assert.AreEqual("1",
                                _xmlDoc.SelectSingleNode(@"results/features/feature").Attributes[
                                    "scenariosPending"].Value);
            }

            [Test]
            public void FeatureNodeShouldHaveOneScenarioNodePerScenarioInStory()
            {
                var node = _xmlDoc.SelectNodes(@"//features/feature[@name='S1']/scenarios/scenario");
                Assert.IsNotNull(node);
                Assert.AreEqual(3, node.Count);
            }

            [Test]
            public void ScenarioNodeShouldHaveStepSubnodes()
            {
                var nodes = _xmlDoc.SelectNodes(@"//feature[@name='S1']/scenarios/scenario[@name='SC1']/step");
                Assert.IsNotNull(nodes);
                Assert.AreEqual(3, nodes.Count);
            }

            [Test]
            public void ShouldHaveLinebreaksBetweenNodes()
            {
                var memStream = new MemoryStream();
                var listener = NBehave.EventListeners.EventListeners.XmlWriterEventListener(memStream);

                ConfigurationNoAppDomain
                    .New
                    .SetScenarioFiles(_feature)
                    .SetAssemblies(new[] {GetType().Assembly.Location})
                    .SetEventListener(listener)
                    .Build()
                    .Run();

                memStream.Seek(0, 0);
                var xmlAsText = new StreamReader(memStream);
                var xml = xmlAsText.ReadToEnd();
                StringAssert.Contains(">" + Environment.NewLine + "<", xml);
            }

            [Test]
            public void ShouldHaveFailureChildNodeInFailedActionStep()
            {
                var node = _xmlDoc.SelectSingleNode(@"//scenario[@name='FailingScenario']/step[@outcome='failed']/failure");
                Assert.IsNotNull(node);
                StringAssert.Contains("outcome failed", node.InnerText);
            }
        }

        [TestFixture]
        public class WhenCreatingXmlForScenarioWithExamples : XmlOutputEventListenerSpec
        {
            public override void Setup()
            {
                _feature = new[] {TestFeatures.ScenarioWithExamples};
                base.Setup();
            }

            [Test]
            public void ShouldHaveXmlInTheSpecifiedXmlWriter()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results"));
            }

            [Test]
            public void ShouldHaveOneScenario()
            {
                var outcome = _xmlDoc.SelectSingleNode("results").Attributes["scenarios"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(1));
            }

            [Test]
            public void ScenarioNodeShouldHaveActionStepSubnodes()
            {
                string[] expectedNodes = {
                                             "Given a string [str]",
                                             "When string is ecco'ed",
                                             "Then you should see [strOut]"
                                         };

                var nodes = _xmlDoc.SelectNodes(@"//feature[@name='Example']/scenarios/scenario[@name='SC1']/step");
                Assert.IsNotNull(nodes);
                Assert.AreEqual(3, nodes.Count);
                for (var i = 0; i < 3; i++)
                {
                    Assert.That(nodes[i].Attributes["name"].Value, Is.EqualTo(expectedNodes[i]));
                }
            }

            [Test]
            public void ScenarioNodeShouldHaveExampleNodes()
            {
                var nodes = _xmlDoc.SelectNodes(@"//feature[@name='Example']/scenarios/scenario[@name='SC1']/examples/example");
                Assert.IsNotNull(nodes);
                Assert.AreEqual(2, nodes.Count);
            }
        }

        [BeforeScenario]
        public void WaitSomeTime()
        {
        }

        [Given(@"a string $str")]
        public void AString(string str)
        {
        }

        [When(@"string is ecco'ed")]
        public void EcchoString()
        {
        }

        [Then(@"you should see $strOut")]
        public void StringOut(string strOut)
        {
        }

        [Given(@"something$")]
        [Given(@"something x$")]
        [Given(@"something two$")]
        public void AGiven()
        {
        }

        [When(@"some event occurs$")]
        [When(@"some event y occurs$")]
        [When(@"some event #2 occurs$")]
        public void AWhen()
        {
        }

        [Then(@"there is some outcome$")]
        [Then(@"there is some outcome #2$")]
        public void AThen()
        {
        }

        [Then(@"there is some failing outcome$")]
        public void AThenFailing()
        {
            throw new Exception("outcome failed");
        }
    }
}
