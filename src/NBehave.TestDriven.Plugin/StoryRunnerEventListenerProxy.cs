using NBehave.Narrator.Framework;
using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    /// <summary>
    /// This class assumed to be tied to TDD.Net implementation
    /// </summary>
    public class StoryRunnerEventListenerProxy : EventListener
    {
        private readonly ITestListener _listener;

        public StoryRunnerEventListenerProxy(ITestListener listener)
        {
            _listener = listener;
        }

        public override void FeatureStarted(string feature)
        {
            _listener.WriteLine("\tFeature: " + feature, Category.Output);
        }

        public override void ScenarioResult(ScenarioResult result)
        {                        
            _listener.WriteLine(string.Format("\t\tScenario: {0} - {1}", result.ScenarioTitle, result.Result), Category.Info);
            _listener.TestFinished(
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
