using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Configuration;
using NBehave.EventListeners;
using NBehave.EventListeners.Xml;
using NBehave.Internal;
using NBehave.Remoting;
using NUnit.Framework;
using TestLib;

namespace NBehave.Specifications
{
    [TestFixture]
    public abstract class RemotableStoryRunnerSpec
    {
        private string tempFileName;

        private IRunner CreateTextRunner(IEnumerable<string> assemblies)
        {
            return CreateTextRunner(assemblies, null);
        }

        private IRunner CreateTextRunner(IEnumerable<string> assemblies, string scenarioText)
        {
            var writer = new StreamWriter(new MemoryStream());
            var listener = new TextWriterEventListener(writer);
            return CreateTextRunner(assemblies, listener, scenarioText);
        }

        private IRunner CreateTextRunner(IEnumerable<string> assemblies, IEventListener listener, string scenarioText)
        {
            var configuration = NBehaveConfiguration.New
                                                    .SetAssemblies(assemblies)
                                                    .SetEventListener(listener);

            if (!String.IsNullOrEmpty(scenarioText))
            {
                // Configure the scenario to get run
                tempFileName = Path.GetTempFileName();
                using (var fileStream = new StreamWriter(File.Create(tempFileName)))
                {
                    fileStream.Write(scenarioText);
                }
                configuration.SetScenarioFiles(new[] { tempFileName });
            }
            return RunnerFactory.CreateTextRunner(configuration);
        }

        private void SetupConfigFile()
        {
            File.WriteAllText(Path.Combine(GetAssemblyLocation(), "TestLib.dll.config"),
                            "<configuration><appSettings><add key=\"foo\" value=\"bar\" /></appSettings></configuration>");
        }

        private void DeleteConfigFile()
        {
            File.Delete(Path.Combine(GetAssemblyLocation(), "TestLib.dll.config"));
            if (!String.IsNullOrEmpty(tempFileName))
                File.Delete(tempFileName);
        }

        private string GetAssemblyLocation()
        {
            var uri = new Uri(typeof(ConfigFileActionSteps).Assembly.CodeBase);
            return Path.GetDirectoryName(uri.LocalPath);
        }

        [TestFixture]
        public class When_creating_a_runner_with_config_file : RemotableStoryRunnerSpec
        {
            private IRunner runner;

            [SetUp]
            public void SetUp()
            {
                SetupConfigFile();
                runner = CreateTextRunner(new[] { "TestLib.dll" });
            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            [Test]
            public void Should_construct_runner_suited_for_remoting()
            {
                Assert.IsInstanceOf(typeof(AppDomainRunner), runner);
            }
        }

        [TestFixture]
        public class When_running_plain_text_scenarios_with_config_file : RemotableStoryRunnerSpec
        {
            private IRunner runner;
            private FeatureResults results;

            [SetUp]
            public void SetUp()
            {
                var scenarioText = "Feature: Config file support" + Environment.NewLine +
                                    "Scenario: Reading values from a config file" + Environment.NewLine +
                                    "Given an assembly with a matching configuration file" + Environment.NewLine +
                                    "When the value of setting foo is read" + Environment.NewLine +
                                    "Then the value should be bar";
                SetupConfigFile();
                runner = CreateTextRunner(new[] { "TestLib.dll" }, scenarioText);

                results = runner.Run();
            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            [Test]
            public void Should_read_values_from_the_appropriate_config_file()
            {
                Assert.AreEqual(1, results.NumberOfPassingScenarios);
                Assert.AreEqual(0, results.NumberOfFailingScenarios);
            }
        }

        [TestFixture]
        public class When_running_failing_plain_text_scenarios_with_config_file : RemotableStoryRunnerSpec
        {
            private IRunner runner;
            private FeatureResults results;

            [SetUp]
            public void SetUp()
            {
                var scenarioText = "Feature: Config file support" + Environment.NewLine +
                                    "Scenario: Reading values from a config file" + Environment.NewLine +
                                    "Given an assembly with a matching configuration file" + Environment.NewLine +
                                    "When the value of setting foo is read" + Environment.NewLine +
                                    "Then the value should be meeble";
                SetupConfigFile();
                runner = CreateTextRunner(new[] { "TestLib.dll" }, scenarioText);

                results = runner.Run();

            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            [Explicit("This test crashes the R# test runner")]
            public void Should_read_values_from_the_appropriate_config_file()
            {
                //WARNING: This test crashes the R# test runner, v 5.1.3000.12 anyway
                Assert.AreEqual(0, results.NumberOfPassingScenarios);
                Assert.AreEqual(1, results.NumberOfFailingScenarios);
            }
        }

        [TestFixture]
        public class When_running_text_scenarios_with_no_feature_and_config_file : RemotableStoryRunnerSpec
        {
            private IRunner _runner;

            [SetUp]
            public void SetUp()
            {
                var scenarioText = "Scenario: Reading values from a config file" + Environment.NewLine +
                                    "Given an assembly with a matching configuration file" + Environment.NewLine +
                                    "When the value of setting foo is read" + Environment.NewLine +
                                    "Then the value should be bar";
                SetupConfigFile();
                _runner = CreateTextRunner(new[] { "TestLib.dll" }, scenarioText);
            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            [Test]
            public void Should_run_scenario()
            {
                var result = _runner.Run();
                Assert.That(result.NumberOfFailingScenarios, Is.EqualTo(0));
                Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(0));
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class When_running_plain_text_scenarios_with_listener_and_config_file : RemotableStoryRunnerSpec
        {
            private IRunner runner;
            private FeatureResults results;
            private XmlDocument xmlOut;
            private const string FeatureTitle = "Scenario runner that can read from its own config file";

            [TestFixtureSetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);

                var scenarioText = "Feature: " + FeatureTitle + Environment.NewLine +
                                    "Scenario: Reading values from a config file" + Environment.NewLine +
                                    "Given an assembly with a matching configuration file" + Environment.NewLine +
                                    "When the value of setting foo is read" + Environment.NewLine +
                                    "Then the value should be bar";

                SetupConfigFile();
                runner = CreateTextRunner(new[] { "TestLib.dll" }, listener, scenarioText);

                results = runner.Run();

                xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                xmlOut.Load(writer.BaseStream);
            }

            [TestFixtureTearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            [Test]
            public void Should_read_values_from_the_appropriate_config_file()
            {
                Assert.AreEqual(1, results.NumberOfPassingScenarios);
            }

            [Test]
            public void Should_find_one_feature()
            {
                var storyNodes = xmlOut.SelectNodes("//feature");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void Should_set_title_of_feature()
            {
                var storyNodes = xmlOut.SelectSingleNode("//feature").Attributes["name"];

                Assert.That(storyNodes.Value, Is.EqualTo(FeatureTitle));
            }

            [Test]
            public void Should_run_one_scenario()
            {
                var scenarioNodes = xmlOut.SelectNodes("//scenario");

                Assert.That(scenarioNodes.Count, Is.EqualTo(1));
            }
        }
    }
}
