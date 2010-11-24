using NBehave.Narrator.Framework;
using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    /// <summary>
    /// This class assumed to be tied to TDD.Net implementation
    /// </summary>
    public class StoryRunnerEventListenerProxy : IEventListener
    {
        private readonly ITestListener _listener;

        public StoryRunnerEventListenerProxy(ITestListener listener)
        {
            _listener = listener;
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
            _listener.WriteLine(string.Format("Theme : {0}", name), Category.Output);
        }

        void IEventListener.FeatureCreated(string feature)
        {
            _listener.WriteLine("\tFeature: " + feature, Category.Output);
        }

        void IEventListener.FeatureNarrative(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        void IEventListener.ScenarioResult(ScenarioResult result)
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

        void IEventListener.RunFinished()
        {
            //results now handled one at a time        
        }

    }
}
