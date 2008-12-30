using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Rhino.Mocks;
using NBehave.Narrator.Framework.EventListeners;
using TestPlainTextAssembly;

using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;


namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class When_using_tokenized_stories
    {
        [Specification]
        public void should_replace_token_with_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.StrictMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is 20");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is $balance", 20, delegate(int accountBalance) { account = new Account(accountBalance); });
            }
        }


        [Specification]
        public void should_not_add_value_to_reused_token()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.StrictMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is 20");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\t\tAnd the account balance is 30");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is $balance", 20, delegate(int accountBalance) { account = new Account(accountBalance); })
                    .And("the account balance is 30");
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


        [Specification]
        public void should_output_full_story_for_dry_run()
        {
            MockRepository mocks = new MockRepository();
            IEventListener listener = mocks.StrictMock<IEventListener>();

            using (mocks.Record())
            {
                listener.RunStarted();
                LastCall.Repeat.Once();
                listener.ThemeStarted("");
                LastCall.IgnoreArguments().Repeat.Once();
                listener.StoryCreated("");
                LastCall.IgnoreArguments().Repeat.Once();
                listener.StoryResults(null);
                LastCall.IgnoreArguments().Repeat.Once();
                listener.StoryMessageAdded("");
                LastCall.IgnoreArguments().Repeat.Any();
                listener.ThemeFinished();
                LastCall.Repeat.Once();
                listener.RunFinished();
                LastCall.Repeat.Once();
            }

            using (mocks.Playback())
            {
                StoryRunner runner = new StoryRunner();

                runner.IsDryRun = true;
                runner.LoadAssembly("TestPlainTextAssembly.dll");
                runner.Run(listener);
            }
        }

    }


    [Context()]
    public class When_running_assembly_with_tokenized_scenario
    {
        private StoryResults results = null;

        [SetUp]
        public void SetupSpec()
        {
            MockRepository mocks = new MockRepository();
            IEventListener evt = mocks.Stub<IEventListener>();
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestPlainTextAssembly.dll");
            results = runner.Run(evt);
        }

        [Specification]
        public void should_find_tokenized_scenario_in_assembly()
        {
            Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
        }

        [Specification]
        public void Should_report_the_number_of_passing_scenarios()
        {
            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
        }
    }

    [Context()]
    public class When_dry_running_asembly_with_tokenized_scenario
    {
        TextWriter writer;

        [SetUp]
        public void SetupSpec()
        {
            writer = new StringWriter();
            IEventListener evt = new TextWriterEventListener(writer);
            StoryRunner runner = new StoryRunner();
            runner.LoadAssembly("TestPlainTextAssembly.dll");
            runner.IsDryRun = true;
            StoryResults results = runner.Run(evt);
        }

        [Specification]
        public void Should_have_given_after_Scenario1()
        {
            writer.Flush();
            string output = writer.ToString();

            int posOfFirstScenario = output.IndexOf("Scenario 1:");
            int posOfFirstGiven = output.IndexOf(Environment.NewLine, posOfFirstScenario) + Environment.NewLine.Length;
            int endOfLinePos = output.IndexOf(Environment.NewLine, posOfFirstGiven);
            string given = output.Substring(posOfFirstGiven, +endOfLinePos - posOfFirstGiven);

            Assert.IsTrue(given.EndsWith("Given my savings account balance is 100"));
        }

        [Specification]
        public void Should_have_given_after_Scenario2()
        {
            writer.Flush();
            string output = writer.ToString();

            int posOfFirstScenario = output.IndexOf("Scenario 2:");
            int posOfFirstGiven = output.IndexOf(Environment.NewLine, posOfFirstScenario) + Environment.NewLine.Length;
            int endOfLinePos = output.IndexOf(Environment.NewLine, posOfFirstGiven);
            string given = output.Substring(posOfFirstGiven, +endOfLinePos - posOfFirstGiven);

            Assert.IsTrue(given.EndsWith("Given my savings account balance is 500"));
        }
    }
}
