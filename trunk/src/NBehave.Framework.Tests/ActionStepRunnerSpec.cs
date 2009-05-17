using System;
using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;


namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionStepRunnerSpec
    {
        public class When_running_plain_text_scenarios : ActionStepRunnerSpec
        {
            private ActionStepRunner _runner;
            [SetUp]
            public void SetUp()
            {
                _runner = new ActionStepRunner();
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Specification]
            public void Should_find_Given_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("Given my name is Axel"), Is.True);
            }

            [Specification]
            public void Should_find_When_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("When I'm greeted"), Is.True);
            }

            [Specification]
            public void Should_find_Then_ActionStep_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("Then I should be greeted with “Hello, Axel!”"), Is.True);
            }

            [Specification]
            public void Should_register_alias_And_for_Given()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("And my name is Axel"), Is.True);
            }

            [Specification]
            public void Should_invoke_action_given_a_token_string()
            {
                _runner.InvokeTokenString("Given my name is Morgan");
            }

            [Specification, ExpectedException(typeof(ArgumentException))]
            public void Should_throw_ArgumentException_if_action_given_in_token_string_doesnt_exist()
            {
                _runner.InvokeTokenString("This doesnt exist");
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
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                StoryResults result = _runner.Run(listener);
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_run_scenarios_in_text_file()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystem.txt" });
                StoryResults result = _runner.Run(listener);
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_get_result_of_running_scenarios_in_text_file()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystem.txt" });
                StoryResults results = _runner.Run(listener);
                Assert.That(results.NumberOfThemes, Is.EqualTo(0));
                Assert.That(results.NumberOfStories, Is.EqualTo(0));
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(1));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Specification]
            public void Should_get_correct_errormessage_from_failed_scenario()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystemFailure.txt" });
                StoryResults results = _runner.Run(listener);
                Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(results.ScenarioResults[0].Message.StartsWith("NUnit.Framework.AssertionException :"), Is.True);
            }

            [Specification]
            public void Should_execute_more_than_one_scenario_in_text_file()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystem_ManyGreetings.txt" });
                StoryResults results = _runner.Run(listener);
                Assert.That(results.NumberOfThemes, Is.EqualTo(0));
                Assert.That(results.NumberOfStories, Is.EqualTo(0));
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Specification]
            public void Should_run_scenario_in_text_file_with_scenario_title()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystemWithScenarioTitle.txt" });
                var results = _runner.Run(listener);

                Assert.That(results.ScenarioResults[0].ScenarioTitle, Is.EqualTo("A simple greeting example"));
                Assert.That(results.ScenarioResults[0].ScenarioResult, Is.EqualTo(ScenarioResult.Passed));
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
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                StoryResults result = _runner.Run(listener);
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
                StoryResults result = _runner.Run(new NullEventListener());
                Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(1));
            }
        }
    }
}
