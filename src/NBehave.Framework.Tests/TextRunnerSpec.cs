using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class TextRunnerSpec
    {
        private TextRunner CreateTextRunner()
        {
            var writer = new StreamWriter(new MemoryStream());
            var listener = new TextWriterEventListener(writer);
            return CreateTextRunner(listener);
        }

        private TextRunner CreateTextRunner(IEventListener listener)
        {
            return new TextRunner(listener);
        }

        [TestFixture]
        public class When_running_plain_text_scenarios : TextRunnerSpec
        {
            private TextRunner _runner;

            [SetUp]
            public void SetUp()
            {
                _runner = CreateTextRunner();
                LoadAssembly();
            }

            private void LoadAssembly()
            {
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Test]
            public void Should_find_Given_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("my name is Axel"), Is.True);
            }

            [Test]
            public void Should_find_When_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I'm greeted"), Is.True);
            }

            [Test]
            public void Should_find_Then_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I should be greeted with “Hello, Axel!”"), Is.True);
            }
            
            [Test]
            public void Should_run_scenarios_in_text_file()
            {
                _runner.Load(new[] { TestFeatures.GreetingSystem });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }
            
            [Test]
            public void Should_get_result_of_running_scenarios_in_text_file()
            {
                _runner.Load(new[] { TestFeatures.GreetingSystem });
                var results = _runner.Run();
                Assert.That(results.NumberOfThemes, Is.EqualTo(0));
                Assert.That(results.NumberOfStories, Is.EqualTo(1));
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(1));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void Should_get_correct_errormessage_from_failed_scenario()
            {
                _runner.Load(new[] { TestFeatures.GreetingSystemFailure });
                var results = _runner.Run();
                Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(results.ScenarioResults[0].Message.StartsWith("NUnit.Framework.AssertionException :"), Is.True);
            }

            [Test]
            public void Should_mark_failing_step_as_failed_in_output()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                LoadAssembly();
                _runner.Load(new[] { TestFeatures.GreetingSystemFailure });
                _runner.Run();
                StringAssert.Contains("Then I should be greeted with “Hello, Scott!” - FAILED", writer.ToString());
            }

            [Test]
            public void Should_execute_more_than_one_scenario_in_text_file()
            {
                var writer = new StreamWriter(new MemoryStream());
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
                _runner.Load(new[] { TestFeatures.GreetingSystemManyGreetings });
                var results = _runner.Run();
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Test]
            public void Should_run_scenario_in_text_file_with_scenario_title()
            {
                _runner.Load(new[] { TestFeatures.GreetingSystemWithScenarioTitle });
                var results = _runner.Run();

                Assert.That(results.ScenarioResults[0].ScenarioTitle, Is.EqualTo("A simple greeting example"));
                Assert.That(results.ScenarioResults[0].Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void Should_run_text_scenario_whith_newlines_in_given()
            {
                _runner.Load(new [] { TestFeatures.GreetingSystemWithNewLinesInGiven });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void Should_set_scenario_pending_if_action_given_in_token_string_doesnt_exist()
            {
                _runner.Load(new [] { TestFeatures.GreetingSystemWithNoActionSteps });
                var result = _runner.Run();
                Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void Should_list_all_pending_actionSteps()
            {
                _runner.Load(new [] { TestFeatures.ShouldListAllPendingActionSteps });
                var result = _runner.Run();
                StringAssert.Contains("No matching Action found for \"Given something that has no ActionStep\"", result.ScenarioResults[0].Message);
                StringAssert.Contains("No matching Action found for \"And something else that has no ActionStep\"", result.ScenarioResults[0].Message);
            }

            [Test]
            public void Should_use_wildcard_and_run_all_scenarios_in_all_matching_text_files()
            {
                _runner.Load(new[] { @"Features\\GreetingSystem*.feature" });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(10));
            }
        }

        [TestFixture]
        public class When_running_with_xml_listener : TextRunnerSpec
        {
            private XmlDocument _xmlOut;

            [SetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new [] { TestFeatures.GreetingSystemWithFailedStep });
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void Should_find_one_failed_actionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='failed']");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void Should_find_two_passed_actionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='passed']");
                Assert.That(storyNodes.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class When_running_plain_text_scenarios_with_xml_listener : TextRunnerSpec
        {
            private const string StoryTitle = "A fancy greeting system";

            private XmlDocument _xmlOut;

            [SetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new[] { TestFeatures.GreetingSystemBDDGuyStory });
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void Should_find_one_story()
            {
                var storyNodes = _xmlOut.SelectNodes("//story");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void Should_set_title_of_story()
            {
                var storyNodes = _xmlOut.SelectSingleNode("//story").Attributes["name"];

                Assert.That(storyNodes.Value, Is.EqualTo(StoryTitle));
            }

            [Test]
            public void Should_run_two_scenarios()
            {
                var scenarioNodes = _xmlOut.SelectNodes("//scenario");

                Assert.That(scenarioNodes.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class When_running_plain_text_scenarios_with_story : TextRunnerSpec
        {
            private FeatureResults _result;
            private StringWriter _messages;

            [SetUp]
            public void SetUp()
            {
                _messages = new StringWriter();
                var listener = new TextWriterEventListener(_messages);
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new []{ TestFeatures.GreetingSystemStory });
                _result = runner.Run();
            }

            [Test]
            public void Should_set_story_title_on_result()
            {
                Assert.That(_result.ScenarioResults[0].FeatureTitle, Is.EqualTo("Greeting system"));
            }

            [Test]
            public void Should_set_narrative_on_result()
            {
                var messages = _messages.ToString();
                StringAssert.Contains("As a project member", messages);
                StringAssert.Contains("I want", messages);
                StringAssert.Contains("So that", messages);
            }

            [Test]
            public void Should_set_scenario_title_on_result()
            {
                Assert.That(_result.ScenarioResults[0].ScenarioTitle, Is.EqualTo("Greeting someone"));
            }
        }

        [TestFixture]
        public class When_running_plain_text_scenarios_with_story_events_raised : TextRunnerSpec
        {
            private IEventListener _listener;

            [TestFixtureSetUp]
            public void Establish_context()
            {
                _listener = MockRepository.GenerateMock<IEventListener>();
                var runner = new TextRunner(_listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new []{ TestFeatures.GreetingSystemStory });
                runner.Run();
            }

            [Test]
            public void Should_get_story_created_event_with_title()
            {
                _listener.AssertWasCalled(l => l.FeatureCreated("Greeting system"));
            }

            [Test]
            public void Should_get_story_narrative()
            {
                var args = _listener.GetArgumentsForCallsMadeOn(l => l.FeatureNarrative(null), opt => opt.IgnoreArguments());
                var arg = args[0][0] as string;
                StringAssert.Contains("As a", arg);
                StringAssert.Contains("I want", arg);
                StringAssert.Contains("So that", arg);
            }

            [Test]
            public void Should_get_scenario_created_event_with_title()
            {
                _listener.AssertWasCalled(l => l.ScenarioCreated("Greeting someone"));
            }
        }
    
        [TestFixture, ActionSteps]
        public class When_running_plain_Text_scenario_in_swedish : TextRunnerSpec
        {
            private TextRunner _runner;

            private readonly Stack<int> _numbers = new Stack<int>();
            private int _calcResult;
            private static bool _givenWasCalled;
            private static bool _whenWasCalled;
            private static bool _thenWasCalled;
            private FeatureResults _featureResults;

            [Given(@"att jag knappat in $number")]
            public void GivenNumber(int number)
            {
                _numbers.Push(number);
                _givenWasCalled = true;
            }

            [When("jag summerar")]
            public void Sum()
            {
                _calcResult = _numbers.Pop() + _numbers.Pop();
                _whenWasCalled = true;
            }

            [Then("ska resultatet vara $sum")]
            public void Result(int sum)
            {
                Assert.AreEqual(sum, _calcResult);
                _thenWasCalled = true;
            }

            [SetUp]
            public void SetUp()
            {
                _runner = new TextRunner(Framework.EventListeners.EventListeners.NullEventListener());
                _runner.LoadAssembly(GetType().Assembly);
                _runner.Load(new[] { TestFeatures.GreetingSystemDifferentLanguage });
                _featureResults = _runner.Run();
            }

            [Test]
            public void Should_run_text_scenario_in_stream()
            {
                Assert.That(_featureResults.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void Given_should_be_called()
            {
                Assert.That(_givenWasCalled, Is.True);
            }

            [Test]
            public void When_should_be_called()
            {
                Assert.That(_whenWasCalled, Is.True);
            }

            [Test]
            public void then_should_be_called()
            {
                Assert.That(_thenWasCalled, Is.True);
            }
        }
    }
}
