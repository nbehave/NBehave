using System;
using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class StoryRunnerSpec
    {
        private IEventListener GetStubbedListener()
        {
            return MockRepository.GenerateStub<IEventListener>();
        }

        [Specification]
        public void Should_find_the_themes_in_the_example_assembly()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(2));
        }

        [Specification]
        public void Should_find_the_stories_in_the_example_assembly()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfStories, Is.EqualTo(4));
        }

        [Specification]
        public void Should_report_the_number_of_scenarios_for_each_story()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfScenariosFound, Is.EqualTo(5));
        }

        [Specification]
        public void Should_report_the_number_of_failed_scenarios()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
        }

        [Specification]
        public void Should_report_the_number_of_pending_scenarios()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(2));
        }

        [Specification]
        public void Should_report_the_number_of_passing_scenarios()
        {
            var runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
        }

        [Specification]
        public void Should_raise_events_for_messages_written()
        {
            var repo = new MockRepository();
            var listener = repo.StrictMock<IEventListener>();

            using (repo.Record())
            {
                listener.RunStarted();
                LastCall.Repeat.Once();
                listener.ThemeStarted("");
                LastCall.IgnoreArguments().Repeat.Twice();
                listener.StoryCreated("");
                LastCall.IgnoreArguments().Repeat.Times(4);
                listener.StoryResults(null);
                LastCall.IgnoreArguments().Repeat.Times(4);
                listener.StoryMessageAdded("");
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ScenarioCreated(null);
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ScenarioMessageAdded(null);
                LastCall.IgnoreArguments().Repeat.AtLeastOnce();
                listener.ThemeFinished();
                LastCall.Repeat.Twice();
                listener.RunFinished();
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                var runner = new StoryRunner();

                runner.LoadAssembly("TestAssembly.dll");
                runner.Run(listener);
            }
        }

        [Specification]
        public void Should_output_full_story_for_dry_run()
        {
            var repo = new MockRepository();
            var listener = repo.StrictMock<IEventListener>();

            using (repo.Record())
            {
                listener.RunStarted();
                LastCall.Repeat.Once();
                listener.ThemeStarted("");
                LastCall.IgnoreArguments().Repeat.Twice();
                listener.StoryCreated("");
                LastCall.IgnoreArguments().Repeat.Times(4);
                listener.StoryResults(null);
                LastCall.IgnoreArguments().Repeat.Times(4);
                listener.StoryMessageAdded("");
                LastCall.IgnoreArguments().Repeat.Times(9);
                listener.ScenarioCreated(null);
                LastCall.IgnoreArguments().Repeat.Times(5);
                listener.ScenarioMessageAdded(null);
                LastCall.IgnoreArguments().Repeat.Times(27);
                listener.ThemeFinished();
                LastCall.Repeat.Twice();
                listener.RunFinished();
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                var runner = new StoryRunner();

                runner.IsDryRun = true;
                runner.LoadAssembly("TestAssembly.dll");
                runner.Run(listener);
            }
        }

        [Specification]
        public void Should_only_add_themes_within_given_namespace_that_matches_namespacefilter()
        {
            var runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter("TestAssembly", ".", ".");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(2));
        }

        [Specification]
        public void Should_not_find_any_themes_within_given_namespace_given_the_namespacefilter_TestAssemblyThatDoesntExists()
        {
            var runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter("TestAssemblyThatDoesntExists", ".", ".");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(0));
        }

        [Specification]
        public void Should_only_match_stories_within_given_methodFilter()
        {
            var runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter(".", ".", "Transfer_to_cash_account");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfStories, Is.EqualTo(1));
        }

        [TestFixture]
        public class When_running_assembly_with_tokenized_scenario
        {
            private StoryResults _results;

            [SetUp]
            public void SetupSpec()
            {
                var mocks = new MockRepository();
                var evt = mocks.Stub<IEventListener>();
                var runner = new StoryRunner();

                runner.LoadAssembly("TestPlainTextAssembly.dll");
                _results = runner.Run(evt);
            }

            [Specification]
            public void should_find_tokenized_scenario_in_assembly()
            {
                Assert.That(_results.NumberOfScenariosFound, Is.EqualTo(2));
            }

            [Specification]
            public void Should_report_the_number_of_passing_scenarios()
            {
                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class When_dry_running_asembly_with_tokenized_scenario
        {
            private string _output;

            [SetUp]
            public void SetupSpec()
            {
                TextWriter writer = new StringWriter();
                IEventListener evt = new TextWriterEventListener(writer);
                var runner = new StoryRunner();
                runner.LoadAssembly("TestPlainTextAssembly.dll");
                runner.IsDryRun = true;
                runner.Run(evt);
                writer.Flush();
                _output = writer.ToString();
            }

            [Specification]
            public void should_have_message_run_started()
            {
                Assert.IsTrue(_output.Contains("run started"));
            }

            [Specification]
            public void should_have_message_story_message_added()
            {
                Assert.IsTrue(_output.Contains("story message added"));
            }

            [Specification]
            public void should_have_message_scenario_message_added()
            {
                Assert.IsTrue(_output.Contains("scenario message added:"));
            }

            [Specification]
            public void should_have_message_theme_finished()
            {
                Assert.IsTrue(_output.Contains("theme finished"));
            }

            [Specification]
            public void should_have_message_scenario_run_finished()
            {
                Assert.IsTrue(_output.Contains("run finished"));
            }

            [Specification]
            public void Should_have_given_after_first_scenario()
            {
                int posOfFirstScenario = _output.IndexOf("scenario created: Savings account is in credit");
                int posOfFirstGiven = _output.IndexOf(Environment.NewLine, posOfFirstScenario) +
                                      Environment.NewLine.Length;
                int endOfLinePos = _output.IndexOf(Environment.NewLine, posOfFirstGiven);
                string given = _output.Substring(posOfFirstGiven, +endOfLinePos - posOfFirstGiven);

                Assert.IsTrue(given.EndsWith("Given my savings account balance is 100"));
            }

            [Specification]
            public void Should_have_given_after_second_scenario()
            {
                int posOfFirstScenario = _output.IndexOf("scenario created: Savings account is in credit with text");
                int posOfFirstGiven = _output.IndexOf(Environment.NewLine, posOfFirstScenario) + Environment.NewLine.Length;
                int endOfLinePos = _output.IndexOf(Environment.NewLine, posOfFirstGiven);
                string given = _output.Substring(posOfFirstGiven, +endOfLinePos - posOfFirstGiven);

                StringAssert.EndsWith("Given my savings account balance is 50", given);
            }
        }

        [Specification]
        public void Should_report_invalid_step_in_result()
        {
            var runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter("TestAssembly", "InvalidActionSpecs", "Invalid_action");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
            Assert.That(results.ScenarioResults[0].Message, Is.EqualTo("Action missing for action 'An invalid action'."));
        }
    }
}