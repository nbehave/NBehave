using System;
using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepRunnerSpecification
    {
        public class When_running_plain_text_scenarios : ActionStepRunnerSpecification
        {
            private ActionStepRunner _runner;
            [SetUp]
            public void SetUp()
            {
                _runner = new ActionStepRunner();
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Test]
            public void Should_find_all_ActionSteps_in_assembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("Given my name is Axel"), Is.True);
                Assert.That(_runner.ActionCatalog.ActionExists("When I'm greeted"), Is.True);
                Assert.That(_runner.ActionCatalog.ActionExists("Then I should be greeted with “Hello, Axel!”"), Is.True);
            }

            [Test]
            public void Should_register_alias_And_for_Given()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("Given my name is Axel"), Is.True);
                Assert.That(_runner.ActionCatalog.ActionExists("And my name is Axel"), Is.True);
            }

            [Test]
            public void Should_invoke_action_given_a_token_string()
            {
                _runner.InvokeTokenString("Given my name is Morgan");
            }

            [Test, ExpectedException(typeof(ArgumentException))]
            public void Should_throw_ArgumentException_if_action_given_in_token_string_doesnt_exist()
            {
                _runner.InvokeTokenString("This doesnt exist");
            }

            [Test]
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
                _runner.Run(listener);
                var output = writer.ToString();
                Assert.That(output.IndexOf("Given my name is Morgan"), Is.GreaterThan(0));
            }

            [Test]
            public void Should_run_scenarios_in_text_file()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystem.txt" });
                _runner.Run(listener);
                var output = writer.ToString();
                Assert.That(output.IndexOf("story message added: Given my name is Morgan"), Is.GreaterThan(0));
                Assert.That(output.IndexOf("story message added: When I'm greeted"), Is.GreaterThan(0));
                Assert.That(output.IndexOf("story message added: Then I should be greeted with “Hello, Morgan!”"), Is.GreaterThan(0));
            }

            [Test]
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

            [Test]
            public void Should_get_correct_errormessage_from_failed_scenario()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystemFailure.txt" });
                StoryResults results = _runner.Run(listener);
                Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(results.ScenarioResults[0].Message.StartsWith("NUnit.Framework.AssertionException :"), Is.True);
            }

            [Test]
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

            [Test]
            public void Should_run_scenario_in_text_file_with_scenario_title()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new[] { @"GreetingSystemWithScenarioTitle.txt" });
                var results = _runner.Run(listener);
                var output = writer.ToString();

                Assert.That(results.ScenarioResults[0].ScenarioTitle, Is.EqualTo("A simple greeting example"));
                Assert.That(results.ScenarioResults[0].ScenarioResult, Is.EqualTo(ScenarioResult.Passed));
            }
        }
    }
}
