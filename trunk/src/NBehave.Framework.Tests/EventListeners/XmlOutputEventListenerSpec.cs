using System;
using System.Text;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;
using System.Xml;
using System.IO;


namespace NBehave.Narrator.Framework.Specifications.EventListeners
{

    [TestFixture]
    public class When_Story_is_running_with_xml_listener
    {
        private MemoryStream _memStream;
        protected XmlWriter _xmlReader;
        protected IEventListener _listener;
        protected XmlDocument _xmlDoc;

        protected void StoryRun(XmlWriter xr)
        {
            _listener = new XmlOutputEventListener(xr);
            _listener.RunStarted();
            _listener.ThemeStarted("a theme");
            _listener.StoryCreated("This seems brittle");
            _listener.StoryMessageAdded("As a savings account holder");
            _listener.StoryMessageAdded("I want to transfer money from my savings account");
            _listener.StoryMessageAdded("So that I can get cash easily from an ATM");

            _listener.ScenarioCreated("Savings account is in credit");
            _listener.ScenarioMessageAdded("Given my savings account balance is: 100");
            _listener.ScenarioMessageAdded("And my cash account balance is: 10");
            _listener.ScenarioMessageAdded("When I transfer to cash account: 20");
            _listener.ScenarioMessageAdded("Then my savings account balance should be: 80");
            _listener.ScenarioMessageAdded("And my cash account balance should be: 30");

            var results = new StoryResults();
            var scenarioResults = new ScenarioResults("This seems brittle", "Savings account is in credit", ScenarioResult.Passed);
            results.AddResult(scenarioResults);


            _listener.ScenarioCreated("Savings account is in credit 2");
            _listener.ScenarioMessageAdded("Given my savings account balance is: 200");
            _listener.ScenarioMessageAdded("And my cash account balance is: 20");
            _listener.ScenarioMessageAdded("When I transfer to cash account: 40");
            _listener.ScenarioMessageAdded("Then my savings account balance should be: 180");
            _listener.ScenarioMessageAdded("And my cash account balance should be: 60");
            scenarioResults = new ScenarioResults("This seems brittle", "Savings account is in credit 2", ScenarioResult.Passed);
            results.AddResult(scenarioResults);
            _listener.StoryResults(results);

            _listener.StoryCreated("This seems brittle 2");
            _listener.StoryMessageAdded("As a savings account holder");
            _listener.StoryMessageAdded("I want to transfer money from my savings account");
            _listener.StoryMessageAdded("So that I can get cash easily from an ATM");

            _listener.ScenarioCreated("Savings account is in credit");
            _listener.ScenarioMessageAdded("Given my savings account balance is: 300");
            _listener.ScenarioMessageAdded("And my cash account balance is: 30");
            _listener.ScenarioMessageAdded("When I transfer to cash account: 25");
            _listener.ScenarioMessageAdded("Then my savings account balance should be: 275");
            _listener.ScenarioMessageAdded("And my cash account balance should be: 55");
            scenarioResults = new ScenarioResults("This seems brittle 2", "Savings account is in credit", ScenarioResult.Failed);
            results.AddResult(scenarioResults);
            _listener.StoryResults(results);
            _listener.ThemeFinished();
            _listener.RunFinished();
        }

        [SetUp]
        public void Setup()
        {
            _memStream = new MemoryStream();
            StoryRun(new XmlTextWriter(_memStream, Encoding.UTF8));
            _xmlDoc = new XmlDocument();
            _memStream.Seek(0, 0);
            _xmlDoc.Load(_memStream);
        }


        [Test]
        public void Should_write_xml_to_specified_xml_writer()
        {
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results"));
        }

        [Test]
        public void Should_write_date_and_time_attributes_to_results_root_node()
        {
            Assert.AreEqual(DateTime.Today.ToShortDateString(), _xmlDoc.SelectSingleNode("results").Attributes["date"].Value);
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["time"].Value);
        }

        [Test]
        public void Should_write_name_and_version_attributes_to_results_root_node()
        {
            Assert.IsTrue(_xmlDoc.SelectSingleNode("results").Attributes["name"].Value.Trim().StartsWith("NBehave"));
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results").Attributes["version"].Value);
        }

        [Test]
        public void Xml_should_have_a_story_with_a_name_attribute()
        {
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results/theme/stories/story"));
            Assert.AreEqual("This seems brittle", _xmlDoc.SelectSingleNode("results/theme/stories/story").Attributes["name"].Value);
        }

        [Test]
        public void Xml_should_have_a_narrative_child_to_story()
        {
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results/theme/stories/story/narrative"));
            Assert.AreEqual("As a savings account holder" + Environment.NewLine +
                            "I want to transfer money from my savings account" + Environment.NewLine +
                            "So that I can get cash easily from an ATM" + Environment.NewLine,
                            _xmlDoc.SelectSingleNode("results/theme/stories/story/narrative").InnerText);
        }

        [Test]
        public void Xml_should_summary_in_root_node()
        {
            Assert.AreEqual("1", _xmlDoc.SelectSingleNode(@"results").Attributes["themes"].Value);
            Assert.AreEqual("2", _xmlDoc.SelectSingleNode(@"results").Attributes["stories"].Value);
            Assert.AreEqual("3", _xmlDoc.SelectSingleNode(@"results").Attributes["scenarios"].Value);
            Assert.AreEqual("1", _xmlDoc.SelectSingleNode(@"results").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", _xmlDoc.SelectSingleNode(@"results").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Theme_nodes_should_contain_attribute_about_execution_time()
        {
            Assert.IsNotNull(_xmlDoc.SelectSingleNode("results/theme").Attributes["time"].Value);
        }

        [Test]
        public void Theme_nodes_should_have_summary()
        {
            Assert.AreEqual("2", _xmlDoc.SelectSingleNode(@"results/theme").Attributes["stories"].Value);
            Assert.AreEqual("3", _xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenarios"].Value);
            Assert.AreEqual("1", _xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", _xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Story_nodes_should_have_summary()
        {
            Assert.AreEqual("2", _xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenarios"].Value);
            Assert.AreEqual("0", _xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", _xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Should_have_one_scenario_node_per_scenario_in_story()
        {
            var node = _xmlDoc.SelectNodes(@"results/theme/stories/story/scenarios/scenario");
            Assert.IsNotNull(node);
            Assert.AreEqual(3, node.Count);
        }
    }
}
