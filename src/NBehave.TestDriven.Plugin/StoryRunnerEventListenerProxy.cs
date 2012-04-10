using NBehave.Narrator.Framework;
using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    /// <summary>
    /// This class assumed to be tied to TDD.Net implementation
    /// </summary>
    public class StoryRunnerEventListenerProxy : EventListener
    {
        private readonly ITestListener listener;

        public StoryRunnerEventListenerProxy(ITestListener listener)
        {
            this.listener = listener;
        }

        public override void FeatureStarted(Feature feature)
        {
            listener.WriteLine("\tFeature: " + feature.Title, Category.Output);
        }

        public override void ScenarioFinished(ScenarioResult result)
        {                        
            listener.WriteLine(string.Format("\t\tScenario: {0} - {1}", result.ScenarioTitle, result.Result), Category.Info);
            listener.TestFinished(
                new TestResult
                {
                    Name = result.ScenarioTitle,
                    Message = result.Result.Message,
                    State = result.Result.ToTestState(),
                    StackTrace = result.StackTrace
                }); 
        }
    }
}
