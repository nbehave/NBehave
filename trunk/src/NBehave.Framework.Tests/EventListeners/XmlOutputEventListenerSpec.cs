using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using System.Xml;
using System.IO;


namespace NBehave.Narrator.Framework.Specifications.EventListeners
{

    [TestFixture]
    public class When_Story_is_running_with_xml_listener
    {
        private MemoryStream memStream;
        protected XmlWriter xmlReader;
        protected IEventListener listener;
        protected XmlDocument xmlDoc;

        protected void StoryRun(XmlWriter xr)
        {
            listener = new XmlOutputEventListener(xr);
            listener.RunStarted();
            listener.ThemeStarted("a theme");
            listener.StoryCreated("This seems brittle");
            listener.StoryMessageAdded("Story: This seems brittle");
            listener.StoryMessageAdded("Narrative:");
            listener.StoryMessageAdded("As a savings account holder");
            listener.StoryMessageAdded("I want to transfer money from my savings account");
            listener.StoryMessageAdded("So that I can get cash easily from an ATM");
            listener.StoryMessageAdded("Scenario 1: Savings account is in credit");
            listener.StoryMessageAdded("Given my savings account balance is: 100");
            listener.StoryMessageAdded("And my cash account balance is: 10");
            listener.StoryMessageAdded("When I transfer to cash account: 20");
            listener.StoryMessageAdded("Then my savings account balance should be: 80");
            listener.StoryMessageAdded("And my cash account balance should be: 30");

            StoryResults results = new StoryResults();
            ScenarioResults scenarioResults = new ScenarioResults("This seems brittle", "Savings account is in credit", ScenarioResult.Passed);
            results.AddResult(scenarioResults);


            listener.StoryMessageAdded("Scenario 2: Savings account is in credit 2");
            listener.StoryMessageAdded("Given my savings account balance is: 200");
            listener.StoryMessageAdded("And my cash account balance is: 20");
            listener.StoryMessageAdded("When I transfer to cash account: 40");
            listener.StoryMessageAdded("Then my savings account balance should be: 180");
            listener.StoryMessageAdded("And my cash account balance should be: 60");
            scenarioResults = new ScenarioResults("This seems brittle", "Savings account is in credit 2", ScenarioResult.Passed);
            results.AddResult(scenarioResults);
            listener.StoryResults(results);

            listener.StoryCreated("This seems brittle 2");
            listener.StoryMessageAdded("Story: This seems brittle 2");
            listener.StoryMessageAdded("Narrative:");
            listener.StoryMessageAdded("As a savings account holder");
            listener.StoryMessageAdded("I want to transfer money from my savings account");
            listener.StoryMessageAdded("So that I can get cash easily from an ATM");

            listener.StoryMessageAdded("Scenario 1: Savings account is in credit");
            listener.StoryMessageAdded("Given my savings account balance is: 300");
            listener.StoryMessageAdded("And my cash account balance is: 30");
            listener.StoryMessageAdded("When I transfer to cash account: 25");
            listener.StoryMessageAdded("Then my savings account balance should be: 275");
            listener.StoryMessageAdded("And my cash account balance should be: 55");
            scenarioResults = new ScenarioResults("This seems brittle 2", "Savings account is in credit", ScenarioResult.Failed);
            results.AddResult(scenarioResults);
            listener.StoryResults(results);
            listener.ThemeFinished();
            listener.RunFinished();
        }

        [SetUp]
        public void Setup()
        {
            memStream = new MemoryStream();
            StoryRun(new XmlTextWriter(memStream, Encoding.UTF8));
            xmlDoc = new XmlDocument();
            memStream.Seek(0, 0);
            xmlDoc.Load(memStream);
        }


        [Test]
        public void Should_write_xml_to_specified_xml_writer()
        {
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results"));
        }

        [Test]
        public void Should_write_date_and_time_attributes_to_results_root_node()
        {
            Assert.AreEqual(DateTime.Today.ToShortDateString(), xmlDoc.SelectSingleNode("results").Attributes["date"].Value);
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results").Attributes["time"].Value);
        }

        [Test]
        public void Should_write_name_and_version_attributes_to_results_root_node()
        {
            Assert.IsTrue(xmlDoc.SelectSingleNode("results").Attributes["name"].Value.Trim().StartsWith("NBehave"));
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results").Attributes["version"].Value);
        }

        [Test]
        public void Xml_should_have_a_story_with_a_name_attribute()
        {
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results/theme/stories/story"));
            Assert.AreEqual("This seems brittle", xmlDoc.SelectSingleNode("results/theme/stories/story").Attributes["name"].Value);
        }

        [Test]
        public void Xml_should_have_a_narrative_child_to_story()
        {
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results/theme/stories/story/narrative"));
            Assert.AreEqual("As a savings account holder" + Environment.NewLine +
                            "I want to transfer money from my savings account" + Environment.NewLine +
                            "So that I can get cash easily from an ATM",
                            xmlDoc.SelectSingleNode("results/theme/stories/story/narrative").InnerText);
        }

        [Test]
        public void Xml_should_summary_in_root_node()
        {
            Assert.AreEqual("1", xmlDoc.SelectSingleNode(@"results").Attributes["themes"].Value);
            Assert.AreEqual("2", xmlDoc.SelectSingleNode(@"results").Attributes["stories"].Value);
            Assert.AreEqual("3", xmlDoc.SelectSingleNode(@"results").Attributes["scenarios"].Value);
            Assert.AreEqual("1", xmlDoc.SelectSingleNode(@"results").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", xmlDoc.SelectSingleNode(@"results").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Theme_nodes_should_contain_attribute_about_execution_time()
        {
            Assert.IsNotNull(xmlDoc.SelectSingleNode("results/theme").Attributes["time"].Value);
        }

        [Test]
        public void Theme_nodes_should_have_summary()
        {
            Assert.AreEqual("2", xmlDoc.SelectSingleNode(@"results/theme").Attributes["stories"].Value);
            Assert.AreEqual("3", xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenarios"].Value);
            Assert.AreEqual("1", xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", xmlDoc.SelectSingleNode(@"results/theme").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Story_nodes_should_have_summary()
        {
            Assert.AreEqual("2", xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenarios"].Value);
            Assert.AreEqual("0", xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenariosFailed"].Value);
            Assert.AreEqual("0", xmlDoc.SelectSingleNode(@"results/theme/stories/story").Attributes["scenariosPending"].Value);
        }

        [Test]
        public void Should_have_one_scenario_node_per_scenario_in_story()
        {
            Assert.AreEqual(3, xmlDoc.SelectNodes(@"results/theme/stories/story/scenarios/scenario").Count);
        }
    }
}
