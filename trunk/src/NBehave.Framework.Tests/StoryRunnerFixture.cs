using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class StoryRunnerFixture
    {
        private IEventListener GetStubbedListener()
        {
            return MockRepository.GenerateStub<IEventListener>();
        }

        [Test]
        public void Should_find_the_themes_in_the_example_assembly()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(1));
        }

        [Test]
        public void Should_find_the_stories_in_the_example_assembly()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfStories, Is.EqualTo(3));
        }

        [Test]
        public void Should_report_the_number_of_scenarios_for_each_story()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfScenariosFound, Is.EqualTo(4));
        }

        [Test]
        public void Should_report_the_number_of_failed_scenarios()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
        }

        [Test]
        public void Should_report_the_number_of_pending_scenarios()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
        }

        [Test]
        public void Should_report_the_number_of_passing_scenarios()
        {
            StoryRunner runner = new StoryRunner();

            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
        }

        [Test]
        public void Should_raise_events_for_messages_written()
        {
            MockRepository repo = new MockRepository();
            IEventListener listener = repo.StrictMock<IEventListener>();

            using (repo.Record())
            {
                listener.RunStarted();
                LastCall.Repeat.Once();
                listener.ThemeStarted("");
                LastCall.IgnoreArguments().Repeat.Once();
                listener.StoryCreated("");
                LastCall.IgnoreArguments().Repeat.Times(3);
                listener.StoryResults(null);
                LastCall.IgnoreArguments().Repeat.Times(3);
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

            using (repo.Playback())
            {
                StoryRunner runner = new StoryRunner();

                runner.LoadAssembly("TestAssembly.dll");
                runner.Run(listener);
            }
        }

        [Test]
        public void Should_output_full_story_for_dry_run()
        {
            var repo = new MockRepository();
            var listener = repo.StrictMock<IEventListener>();

            using (repo.Record())
            {
                listener.RunStarted();
                LastCall.Repeat.Once();
                listener.ThemeStarted("");
                LastCall.IgnoreArguments().Repeat.Once();
                listener.StoryCreated("");
                LastCall.IgnoreArguments().Repeat.Times(3);
                listener.StoryResults(null);
                LastCall.IgnoreArguments().Repeat.Times(3);
                listener.StoryMessageAdded("");
                LastCall.IgnoreArguments().Repeat.Times(9);
                listener.ScenarioCreated(null);
                LastCall.IgnoreArguments().Repeat.Times(4);
                listener.ScenarioMessageAdded(null);
                LastCall.IgnoreArguments().Repeat.Times(26);
                listener.ThemeFinished();
                LastCall.Repeat.Once();
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

        [Test]
        public void Should_only_add_themes_within_given_namespace_that_matches_namespacefilter()
        {
            StoryRunner runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter("TestAssembly", ".", ".");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(1));
        }

        [Test]
        public void Should_not_find_any_themes_within_given_namespace_given_the_namespacefilter_TestAssemblyThatDoesntExists()
        {
            StoryRunner runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter("TestAssemblyThatDoesntExists", ".", ".");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfThemes, Is.EqualTo(0));
        }

        [Test]
        public void Should_only_match_stories_within_given_methodFilter()
        {
            StoryRunner runner = new StoryRunner();

            runner.StoryRunnerFilter = new StoryRunnerFilter(".", ".", "Transfer_to_cash_account");
            runner.LoadAssembly("TestAssembly.dll");
            StoryResults results = runner.Run(GetStubbedListener());

            Assert.That(results.NumberOfStories, Is.EqualTo(1));
        }
    }
}