using System;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Rhino.Mocks;
using NBehave.Narrator.Framework.EventListeners;

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
            string actual = string.Empty;
            Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

            new Story("Transfer to cash account")
                .WithScenario("Account has sufficient funds")
                .Given("the account balance is $balance", 20, accountBalance => { });

            Assert.AreEqual(
                               "Given: Given the account balance is 20" + Environment.NewLine
                               , actual);
        }

        [Specification]
        public void should_not_add_value_to_reused_token()
        {
            string actual = string.Empty;
            Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

            new Story("Transfer to cash account")
                .WithScenario("Account has sufficient funds")
                .Given("the account balance is $balance", 20, accountBalance => { })
                .And("the account balance is 30");

            Assert.AreEqual(
                       "Given: Given the account balance is 20" + Environment.NewLine +
                       "And: And the account balance is 30" + Environment.NewLine
                       , actual);

        }

        [Specification]
        public void should_output_full_story_for_dry_run()
        {
            var mocks = new MockRepository();
            var listener = mocks.StrictMock<IEventListener>();

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
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ScenarioCreated(null);
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ScenarioMessageAdded(null);
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ThemeFinished();
                LastCall.Repeat.Once();
                listener.RunFinished();
                LastCall.Repeat.Once();
            }

            using (mocks.Playback())
            {
                var runner = new StoryRunner();

                runner.IsDryRun = true;
                runner.LoadAssembly("TestPlainTextAssembly.dll");
                runner.Run(listener);
            }
        }
    }


    [Context]
    public class When_running_assembly_with_tokenized_scenario
    {
        private StoryResults results;

        [SetUp]
        public void SetupSpec()
        {
            var mocks = new MockRepository();
            var evt = mocks.Stub<IEventListener>();
            var runner = new StoryRunner();

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

    [Context]
    public class When_dry_running_asembly_with_tokenized_scenario
    {
        TextWriter writer;

        [SetUp]
        public void SetupSpec()
        {
            writer = new StringWriter();
            IEventListener evt = new TextWriterEventListener(writer);
            var runner = new StoryRunner();
            runner.LoadAssembly("TestPlainTextAssembly.dll");
            runner.IsDryRun = true;
            runner.Run(evt);
        }

        [Specification]
        public void Should_have_given_after_Scenario1()
        {
            writer.Flush();
            string output = writer.ToString();

            int posOfFirstScenario = output.IndexOf("scenario created: Savings account is in credit");
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

            int posOfFirstScenario = output.IndexOf("scenario created: Savings account is in credit with text");
            int posOfFirstGiven = output.IndexOf(Environment.NewLine, posOfFirstScenario) + Environment.NewLine.Length;
            int endOfLinePos = output.IndexOf(Environment.NewLine, posOfFirstGiven);
            string given = output.Substring(posOfFirstGiven, +endOfLinePos - posOfFirstGiven);

            Assert.IsTrue(given.EndsWith("Given my savings account balance is 500"));
        }
    }
}
