using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class PlainTextSpecification
    {
        public class When_running_plain_text_scenarios : PlainTextSpecification
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
                Assert.That(_runner.ActionCatalog.ActionExists("Given my name Axel"), Is.True);
                Assert.That(_runner.ActionCatalog.ActionExists("When I ask to be greeted"), Is.True);
                Assert.That(_runner.ActionCatalog.ActionExists("Then I should be greeted with “Hello, Axel!”"), Is.True);
            }

            [Test]
            public void Should_invoke_action_given_a_token_string()
            {
                _runner.InvokeTokenString("Given my name Morgan");
            }

            [Test, ExpectedException(typeof(ArgumentException))]
            public void Should_throw_ArgumentException_if_action_given_in_token_string_doesnt_exist()
            {
                _runner.InvokeTokenString("This doesnt exist");
            }

            // Test fails due to a bug in ActionCatalog, it extracts the name Morgan!” in the Then part.
            [Test]
            public void Should_run_text_scenario_in_stream()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.WriteLine("Given my name Morgan");
                sr.WriteLine("When I ask to be greeted");
                sr.WriteLine("Then I should be greeted with “Hello, Morgan!”");
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                _runner.Load(ms);
                _runner.Run(listener);
                var output = writer.ToString();
                Assert.That(output.IndexOf("Given my name Morgan"), Is.GreaterThan(0));
            }

            [Test]
            public void Should_run_scenarios_in_files()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner.Load(new string[] { @"GreetingSystem.txt" });
                _runner.Run(listener);
                var output = writer.ToString();
            }

        }

        //[Specification()]
        //public void should_run_plaintext_story()
        //{
        //    string scenario = "Savings account is in credit" + Environment.NewLine +
        //                      "Given my savings account balance is 50" + Environment.NewLine +
        //                      "And my cash account balance is 80" + Environment.NewLine +
        //                      "When I transfer 20 to cash account" + Environment.NewLine +
        //                      "Then my savings account balance should be 30" + Environment.NewLine +
        //                      "And my cash account balance should be 100" + Environment.NewLine;

        //    PlainTextScenario builder = new PlainTextScenario(AccountSpecs.StoryTitle, scenario);

        //    Assert.That(builder.Given.Length, Is.EqualTo(2));
        //    Assert.That(builder.When, Is.EqualTo("When I transfer 20 to cash account"));
        //    Assert.That(builder.Then.Length, Is.EqualTo(2));
        //}
    }
}
