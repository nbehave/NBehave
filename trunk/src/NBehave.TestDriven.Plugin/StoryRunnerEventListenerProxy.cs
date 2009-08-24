using System;
using System.Linq;
using NBehave.Narrator.Framework;
using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    public class StoryRunnerEventListenerProxy : IEventListener
    {
        private readonly ITestListener _listener;
        private string _story;

        public StoryRunnerEventListenerProxy(ITestListener listener)
        {
            _listener = listener;
        }

        #region IEventListener Members

               void IEventListener.RunStarted()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
            _listener.WriteLine(string.Format("Theme : {0}", name), Category.Output);
        }

        void IEventListener.StoryCreated(string story)
        {
            _story = story;
            _listener.WriteLine("\tStory: " + story, Category.Output);
        }

        void IEventListener.StoryMessageAdded(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        void IEventListener.RunFinished()
        {
        }

        void IEventListener.StoryResults(StoryResults results)
        {
            var resultsFromStory = from r in results.ScenarioResults
                                   where r.StoryTitle == _story
                                   select new
                                   {
                                       Message = r.Result.ToString(),
                                       Name = r.ScenarioTitle,
                                       StackTrace = r.StackTrace,
                                       State = r.Result,
                                   };

            foreach (var result in resultsFromStory)
            {
                var testResult = new TestResult
                    {
                        Message = result.Message,
                        Name = result.Name,
                        StackTrace = result.StackTrace,
                        State = GetTestResultState(result.State),
                        TotalTests = 1
                    };
                _listener.WriteLine(string.Format("\t\tScenario: {0} - {1}", result.Name, result.State), Category.Info);
                _listener.TestFinished(testResult);
            }
        }

        private TestState GetTestResultState(Result result)
        {
            if (result.GetType() == typeof (Passed))
                return TestState.Passed;
            if (result.GetType() == typeof (Failed))
                return TestState.Failed;
            if (result.GetType() == typeof (Pending))
                return TestState.Ignored;
            throw new NotSupportedException(string.Format("Result {0} isn't supported.", result));
        }

        #endregion

    }
}
