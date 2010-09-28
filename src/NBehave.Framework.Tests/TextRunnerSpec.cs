using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class TextRunnerSpec
    {
        private FeatureResults RunAction(string actionStep, TextRunner runner)
        {
            runner.Load(actionStep.ToStream());
            return runner.Run();
        }

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

        [Context]
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

            [Specification]
            public void Should_find_Given_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("my name is Axel"), Is.True);
            }

            [Specification]
            public void Should_find_When_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I'm greeted"), Is.True);
            }

            [Specification]
            public void Should_find_Then_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I should be greeted with “Hello, Axel!”"), Is.True);
            }

            [Specification]
            public void Should_run_text_scenario_in_stream()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Given my name is Morgan");
                sr.WriteLine("When I'm greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Morgan!”");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
                FeatureResults result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }
            
            [Specification]
            public void Should_run_scenarios_in_text_file()
            {
                _runner.Load(new[] { @"GreetingSystem.txt" });
                FeatureResults result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }
            
            [Specification]
            public void Should_get_result_of_running_scenarios_in_text_file()
            {
                _runner.Load(new[] { @"GreetingSystem.txt" });
                FeatureResults results = _runner.Run();
                Assert.That(results.NumberOfThemes, Is.EqualTo(0));
                Assert.That(results.NumberOfStories, Is.EqualTo(1));
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(1));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_get_correct_errormessage_from_failed_scenario()
            {
                _runner.Load(new[] { @"GreetingSystemFailure.txt" });
                FeatureResults results = _runner.Run();
                Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(results.ScenarioResults[0].Message.StartsWith("NUnit.Framework.AssertionException :"), Is.True);
            }

            [Specification]
            public void Should_mark_failing_step_as_failed_in_output()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                LoadAssembly();
                _runner.Load(new[] { @"GreetingSystemFailure.txt" });
                _runner.Run();
                StringAssert.Contains("Then I should be greeted with “Hello, Scott!” - FAILED", writer.ToString());
            }

            [Specification]
            public void Should_execute_more_than_one_scenario_in_text_file()
            {
                var writer = new StreamWriter(new MemoryStream());
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
                _runner.Load(new[] { @"GreetingSystem_ManyGreetings.txt" });
                FeatureResults results = _runner.Run();
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Specification]
            public void Should_run_scenario_in_text_file_with_scenario_title()
            {
                _runner.Load(new[] { @"GreetingSystemWithScenarioTitle.txt" });
                var results = _runner.Run();

                Assert.That(results.ScenarioResults[0].ScenarioTitle, Is.EqualTo("A simple greeting example"));
                Assert.That(results.ScenarioResults[0].Result, Is.TypeOf(typeof(Passed)));
            }

            [Specification]
            public void Should_run_text_scenario_whith_newlines_in_given()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Given");
                sr.WriteLine("my name is Morgan");
                sr.WriteLine("When I'm greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Morgan!”");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
                FeatureResults result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_set_scenario_pending_if_action_given_in_token_string_doesnt_exist()
            {
                var stream = new MemoryStream();
                var sr = new StreamWriter(stream);
                sr.WriteLine("Given something that has no ActionStep");
                sr.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                _runner.Load(stream);
                FeatureResults result = _runner.Run();
                Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_list_all_pending_actionSteps()
            {
                var stream = new MemoryStream();
                var sr = new StreamWriter(stream);
                sr.WriteLine("Given something that has no ActionStep");
                sr.WriteLine("And something else that has no ActionStep");
                sr.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                _runner.Load(stream);
                FeatureResults result = _runner.Run();
                StringAssert.Contains("No matching Action found for \"Given something that has no ActionStep\"", result.ScenarioResults[0].Message);
                StringAssert.Contains("No matching Action found for \"And something else that has no ActionStep\"", result.ScenarioResults[0].Message);
            }

            [Specification]
            public void Should_use_wildcard_and_run_all_scenarios_in_all_matching_text_files()
            {
                _runner.Load(new[] { @"GreetingSystem*.txt" });
                FeatureResults result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(6));
            }
        }

        [Context]
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


                var stream = CreateScenario();
                runner.Load(stream);
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            private Stream CreateScenario()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Given my name is Morgan");
                sr.WriteLine("When I'm greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Failed step!”");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }

            [Specification]
            public void Should_find_one_failed_actionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='failed']");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Specification]
            public void Should_find_two_passed_actionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='passed']");
                Assert.That(storyNodes.Count, Is.EqualTo(2));
            }
        }

        [Context]
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

                var stream = CreateScenario();
                runner.Load(stream);
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            private Stream CreateScenario()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Story: " + StoryTitle);
                sr.WriteLine("As a bdd guy");
                sr.WriteLine("I want this to work");
                sr.WriteLine("So that I can specify my stories as text");
                sr.WriteLine("Scenario: A greeting");
                sr.WriteLine("Given my name is Morgan");
                sr.WriteLine("When I'm greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Morgan!”");
                sr.WriteLine("Scenario: Another greeting");
                sr.WriteLine("Given my name is Axel");
                sr.WriteLine("When I'm greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Axel!”");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
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

                Assert.That(scenarioNodes.Count, Is.EqualTo(2));
            }
        }

        [Context]
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

                string actionSteps = "Story: Greeting system" + Environment.NewLine +
                    "As a project member" + Environment.NewLine +
                    "I want specs written in a non techie way" + Environment.NewLine +
                    "So that everyone can understand them" + Environment.NewLine +
                    "Scenario: Greeting someone" + Environment.NewLine +
                    "Given my name is Morgan" + Environment.NewLine +
                    "When I'm greeted" + Environment.NewLine +
                    "Then I should be greeted with “Hello, Morgan!”";

                _result = RunAction(actionSteps, runner);
            }

            [Specification]
            public void Should_set_story_title_on_result()
            {
                Assert.That(_result.ScenarioResults[0].FeatureTitle, Is.EqualTo("Greeting system"));
            }

            [Specification]
            public void Should_set_narrative_on_result()
            {
                string messages = _messages.ToString();
                StringAssert.Contains("As a project member", messages);
                StringAssert.Contains("I want", messages);
                StringAssert.Contains("So that", messages);
            }

            [Specification]
            public void Should_set_scenario_title_on_result()
            {
                Assert.That(_result.ScenarioResults[0].ScenarioTitle, Is.EqualTo("Greeting someone"));
            }
        }

        [Context]
        public class When_running_plain_text_scenarios_with_story_events_raised : TextRunnerSpec
        {
            private IEventListener _listener;

            [TestFixtureSetUp]
            public void Establish_context()
            {
                _listener = MockRepository.GenerateMock<IEventListener>();
                var runner = new TextRunner(_listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                string actionSteps = "Story: Greeting system" + Environment.NewLine +
                    "As a project member" + Environment.NewLine +
                    "I want specs written in a non techie way" + Environment.NewLine +
                    "So that everyone can understand them" + Environment.NewLine +
                    "Scenario: Greeting someone" + Environment.NewLine +
                    "Given my name is Morgan" + Environment.NewLine +
                    "When I'm greeted" + Environment.NewLine +
                    "Then I should be greeted with “Hello, Morgan!”";

                var result = RunAction(actionSteps, runner);
            }

            [Specification]
            public void Should_get_story_created_event_with_title()
            {
                _listener.AssertWasCalled(l => l.FeatureCreated("Greeting system"));
            }

            [Specification]
            public void Should_get_story_narrative()
            {
                IList<object[]> args = _listener.GetArgumentsForCallsMadeOn(l => l.FeatureNarrative(null), opt => opt.IgnoreArguments());
                var arg = args[0][0] as string;
                StringAssert.Contains("As a", arg);
                StringAssert.Contains("I want", arg);
                StringAssert.Contains("So that", arg);
            }

            [Specification]
            public void Should_get_scenario_created_event_with_title()
            {
                _listener.AssertWasCalled(l => l.ScenarioCreated("Greeting someone"));
            }
        }
    
        [Context, ActionSteps]
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
                _featureResults = RunScenario();
            }

            private FeatureResults RunScenario()
            {
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("# language: se");
                sr.WriteLine("Givet att jag knappat in 5");
                sr.WriteLine("Och att jag knappat in 4");
                sr.WriteLine("När jag summerar");
                sr.WriteLine("Då ska resultatet vara 9");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
                return _runner.Run();
            }

            [Specification]
            public void Should_run_text_scenario_in_stream()
            {
                Assert.That(_featureResults.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Given_should_be_called()
            {
                Assert.That(_givenWasCalled, Is.True);
            }

            [Specification]
            public void When_should_be_called()
            {
                Assert.That(_whenWasCalled, Is.True);
            }

            [Specification]
            public void then_should_be_called()
            {
                Assert.That(_thenWasCalled, Is.True);
            }
        }
    }
}
