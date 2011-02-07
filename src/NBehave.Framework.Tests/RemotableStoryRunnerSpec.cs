using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NBehave.Narrator.Framework.Remoting;
using NBehave.Narrator.Framework.Text;
using NUnit.Framework;
using Rhino.Mocks;
using TestPlainTextAssembly;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class RemotableStoryRunnerSpec
    {
        private FeatureResults RunAction(string actionStep, IRunner runner)
        {
            runner.Load((IEnumerable<string>) actionStep.ToStream());
            return runner.Run();
        }

        private IRunner CreateTextRunner(IEnumerable<string> assemblies)
        {
            var writer = new StreamWriter(new MemoryStream());
            var listener = new TextWriterEventListener(writer);
            return CreateTextRunner(assemblies, listener);
        }

        private IRunner CreateTextRunner(IEnumerable<string> assemblies, IEventListener listener)
        {
            return RunnerFactory.CreateTextRunner(assemblies, listener);
        }

        private void SetupConfigFile()
        {
            File.WriteAllText(Path.Combine(GetAssemblyLocation(), "TestPlainTextAssembly.dll.config"),
                            "<configuration><appSettings><add key=\"foo\" value=\"bar\" /></appSettings></configuration>");
        }

        private void DeleteConfigFile()
        {
            File.Delete(Path.Combine(GetAssemblyLocation(), "TestPlainTextAssembly.dll.config"));
        }

        private string GetAssemblyLocation()
        {
            var assemblyPath = typeof(ConfigFileActionSteps).Assembly
                                                             .CodeBase
                                                             .Replace("file:///", "");
            return Path.GetDirectoryName(assemblyPath);
        }

        [Context]
        public class When_creating_a_runner_with_config_file : RemotableStoryRunnerSpec
        {
            private IRunner _runner;

            [SetUp]
            public void SetUp()
            {
                SetupConfigFile();
                _runner = CreateTextRunner(new[] { "TestPlainTextAssembly.dll" });
                LoadAssembly();
            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            private void LoadAssembly()
            {
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Specification]
            public void Should_construct_runner_suited_for_remoting()
            {
                Assert.IsInstanceOf(typeof(RemotableStoryRunner), _runner);
            }
        }

        [Context]
        public class When_running_plain_text_scenarios_with_config_file : RemotableStoryRunnerSpec
        {
            private IRunner _runner;
            private FeatureResults _result;

            [SetUp]
            public void SetUp()
            {
                SetupConfigFile();
                _runner = CreateTextRunner(new[] { "TestPlainTextAssembly.dll" });
                LoadAssembly();

                LoadScenario();

                _result = _runner.Run();

            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            private void LoadScenario()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Given an assembly with a matching configuration file");
                sr.WriteLine("When the value of setting foo is read");
                sr.WriteLine("Then the value should be bar");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
            }
            
            private void LoadAssembly()
            {
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Specification]
            public void Should_read_values_from_the_appropriate_config_file()
            {
                Assert.AreEqual(1, _result.NumberOfPassingScenarios);
            }
        }

        [Context]
        public class When_running_plain_text_scenarios_with_listener_and_config_file : RemotableStoryRunnerSpec
        {
            private IRunner _runner;
            private FeatureResults _result;
            private XmlDocument _xmlOut;
            private const string StoryTitle = "Scenario runner that can read from its own config file";

            [SetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);

                SetupConfigFile();
                _runner = CreateTextRunner(new[] { "TestPlainTextAssembly.dll" }, listener);
                LoadAssembly();

                LoadScenario();

                _result = _runner.Run();

                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [TearDown]
            public void TearDown()
            {
                DeleteConfigFile();
            }

            private void LoadScenario()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Story: " + StoryTitle);
                sr.WriteLine("Given an assembly with a matching configuration file");
                sr.WriteLine("When the value of setting foo is read");
                sr.WriteLine("Then the value should be bar");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
            }

            private void LoadAssembly()
            {
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Specification]
            public void Should_read_values_from_the_appropriate_config_file()
            {
                Assert.AreEqual(1, _result.NumberOfPassingScenarios);
            }

            [Specification]
            public void Should_find_one_story()
            {
                var storyNodes = _xmlOut.SelectNodes("//story");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Specification]
            public void Should_set_title_of_story()
            {
                var storyNodes = _xmlOut.SelectSingleNode("//story").Attributes["name"];

                Assert.That(storyNodes.Value, Is.EqualTo(StoryTitle));
            }

            [Specification]
            public void Should_run_two_scenarios()
            {
                var scenarioNodes = _xmlOut.SelectNodes("//scenario");

                Assert.That(scenarioNodes.Count, Is.EqualTo(1));
            }

        }

    }
}