using System;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    public class XmlOutputEventListenerSpec
    {
        [TestFixture]
        public class When_Story_is_running_with_xml_listener
        {
            private XmlDocument _xmlDoc;

            [SetUp]
            public void Setup()
            {
                RunnerBase runner = new StoryRunner();
                runner.LoadAssembly(GetType().Assembly);
                var memStream = new MemoryStream();
                var listener = new XmlOutputEventListener(new XmlTextWriter(memStream, Encoding.UTF8));
                runner.Run(listener);
                _xmlDoc = new XmlDocument();
                memStream.Seek(0, 0);
                _xmlDoc.Load(memStream);
            }

            [Test]
            public void Should_have_xml_in_the_specified_xml_writer()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results"));
            }

            [Test]
            public void Results_node_should_have_date_and_time_attributes()
            {
                Assert.AreEqual(DateTime.Today.ToShortDateString(),
                                _xmlDoc.SelectSingleNode("results").Attributes["date"].Value);
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["time"].Value);
            }

            [Test]
            public void Results_node_hould_have_name_attribute()
            {
                Assert.IsTrue(_xmlDoc.SelectSingleNode("results").Attributes["name"].Value.Trim().StartsWith("NBehave"));
            }

            [Test]
            public void Results_node_should_have_version_attribute()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["version"].Value);
            }

            [Test]
            public void Results_node_should_have_two_themes()
            {
                var outcome = _xmlDoc.SelectSingleNode("results").Attributes["themes"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(2));
            }

            [Test]
            public void Should_have_scenarios_in_results_node()
            {
                var outcome = _xmlDoc.SelectSingleNode("results").Attributes["scenarios"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(6));
            }

            [Test]
            public void Theme_T1_should_have_four_scenarios()
            {
                var outcome = _xmlDoc.SelectSingleNode("results/theme[@name='T1']").Attributes["scenarios"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(4));
            }

            [Test]
            public void Theme_T2_should_have_two_scenarios()
            {
                var outcome = _xmlDoc.SelectSingleNode("results/theme[@name='T2']").Attributes["scenarios"].Value;
                Assert.That(int.Parse(outcome), Is.EqualTo(2));
            }

            [Test]
            public void Theme_nodes_should_contain_attribute_about_execution_time()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("results/theme[@name='T1']").Attributes["time"].Value);
            }

            [Test]
            public void Theme_node_T1_should_have_one_pending_scenarios()
            {
                Assert.AreEqual("1",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']").Attributes["scenariosPending"].
                                    Value);
            }

            [Test]
            public void Theme_node_T1_should_have_zero_failed_scenarios()
            {
                Assert.AreEqual("0",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']").Attributes["scenariosFailed"].
                                    Value);
            }

            [Test]
            public void Theme_node_T2_should_have_zero_pending_scenarios()
            {
                Assert.AreEqual("1",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']").Attributes["scenariosPending"].
                                    Value);
            }

            [Test]
            public void Theme_node_T2_should_have_one_failed_scenarios()
            {
                Assert.AreEqual("0",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']").Attributes["scenariosFailed"].
                                    Value);
            }

            [Test]
            public void Story_node_should_have_a_name_attribute()
            {
                Assert.IsNotNull(_xmlDoc.SelectSingleNode("//story[@name='S1']"));
            }

            [Test]
            public void Story_should_have_a_narrative_child_element()
            {
                Assert.IsNotNull(
                    _xmlDoc.SelectSingleNode("results/theme[@name='T1']/stories/story[@name='S1']/narrative"));
                Assert.AreEqual("As a X1" + Environment.NewLine +
                                "I want Y1" + Environment.NewLine +
                                "So that Z1" + Environment.NewLine,
                                _xmlDoc.SelectSingleNode("results/theme/stories/story/narrative").InnerText);
            }

            [Test]
            public void Story_node_should_have_summary()
            {
                Assert.AreEqual("3",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']/stories/story").Attributes[
                                    "scenarios"].Value);
                Assert.AreEqual("0",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']/stories/story").Attributes[
                                    "scenariosFailed"].Value);
                Assert.AreEqual("1",
                                _xmlDoc.SelectSingleNode(@"results/theme[@name='T1']/stories/story").Attributes[
                                    "scenariosPending"].Value);
            }

            [Test]
            public void Story_node_should_have_one_scenario_node_per_scenario_in_story()
            {
                var node = _xmlDoc.SelectNodes(@"//stories/story[@name='S1']/scenarios/scenario");
                Assert.IsNotNull(node);
                Assert.AreEqual(3, node.Count);
            }

            [Test]
            public void Scenario_node_should_have_actionStep_subnodes()
            {
                var nodes = _xmlDoc.SelectNodes(@"//story[@name='S1']/scenarios/scenario[@name='SC1']/actionStep");
                Assert.IsNotNull(nodes);
                Assert.AreEqual(3, nodes.Count);
            }

            [Test]
            public void Scenarios_child_node_actionStep_should_Not_be_empty_on_pending_scenario()
            {
                var nodes = _xmlDoc.SelectNodes(@"//scenario[@name='PendingScenario']/actionStep");
                Assert.IsNotNull(nodes[0].InnerText);
                StringAssert.AreEqualIgnoringCase("Pending: Im not done yet", nodes[0].Attributes["name"].Value);
            }

            [Test]
            public void Should_have_linebreaks_between_nodes()
            {
                RunnerBase runner = new StoryRunner();
                runner.LoadAssembly(GetType().Assembly);
                var memStream = new MemoryStream();
                var listener = Framework.EventListeners.EventListeners.XmlWriterEventListener(memStream);
                runner.Run(listener);
                memStream.Seek(0, 0);
                var xmlAsText = new StreamReader(memStream);
                string xml = xmlAsText.ReadToEnd();
                StringAssert.Contains(">" + Environment.NewLine + "<", xml);
            }
        
            [Test]
            public void Should_have_failure_child_node_in_failed_actionStep()
            {
            	var node = _xmlDoc.SelectSingleNode(@"//scenario[@name='FailingScenario']/actionStep[@outcome='failed']/failure");
                Assert.IsNotNull(node);
                StringAssert.Contains("outcome failed", node.InnerText);
            }
        }
    }
}