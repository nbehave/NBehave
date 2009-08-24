using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using Rhino.Mocks;
using TestDriven.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;

namespace NBehave.TestDriven.Plugin.Specs
{
    [Context]
    public class When_running_stories_from_testdriven_net
    {
        private delegate bool VerifyTestResult(TestResult result);

        private readonly MockRepository mocks = new MockRepository();

        [Specification]
        public void Should_send_testresult_to_ITestListener()
        {
            const string myStory = "My story";
            const string myScenario = "My scenario";
            var tddNetListener = mocks.StrictMock<ITestListener>();
            StoryResults result = null;
            IEventListener storyRunner = null;

            using (mocks.Record())
            {
                tddNetListener.WriteLine("", Category.Output);
                LastCall.IgnoreArguments().Repeat.Twice();

                tddNetListener.TestFinished(null);
                LastCall.Callback(new VerifyTestResult(r => r.TotalTests == 1 && r.Name == myScenario));

                result = new StoryResults();
                result.AddResult(new ScenarioResult(myStory, myScenario));
                storyRunner = new StoryRunnerEventListenerProxy(tddNetListener);
            }

            using (mocks.Playback())
            {
                storyRunner.StoryCreated(myStory);
                storyRunner.StoryResults(result);
            }
        }
    }
}