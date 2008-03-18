using System.Reflection;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NBehave.TestDriven.Plugin;
using TestDriven.Framework;
using Rhino.Mocks;

using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;
using Rhino.Mocks.Constraints;


namespace Nbehave.TestDriven.Plugin.Specs
{
    [Context()]
    public class When_running_stories_from_testdriven_net
    {
        private delegate bool VerifyTestResult(TestResult result);
        MockRepository mocks = new MockRepository();

        [Specification()]
        public void Should_send_testresult_to_ITestListener()
        {
            const string myStory = "My story";
            const string myScenario = "My scenario";
            ITestListener tddNetListener = mocks.CreateMock<ITestListener>();
            StoryResults result = null;
            IEventListener storyRunner = null;

            using (mocks.Record())
            {
                tddNetListener.WriteLine("", Category.Output);
                LastCall.IgnoreArguments().Repeat.Twice();

                tddNetListener.TestFinished(null);
                LastCall.Callback(new VerifyTestResult(r => { return r.TotalTests == 1 && r.Name == myScenario; }));

                result = new StoryResults();
                result.AddResult(new ScenarioResults(myStory, myScenario));
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
