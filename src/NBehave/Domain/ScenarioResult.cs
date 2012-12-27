using System.Linq;
using System;
using System.Collections.Generic;

namespace NBehave
{
    [Serializable]
    public class ScenarioResult
    {
        private readonly List<StepResult> _actionStepResults;
        private Result _result = new Passed();
        private string _message;
        private string _stackTrace;

        public ScenarioResult(Feature feature, string scenarioTitle)
        {
            FeatureTitle = feature.Title;
            ScenarioTitle = scenarioTitle;
            _actionStepResults = new List<StepResult>();
        }

        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }

        public Result Result
        {
            get { return _result; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string StackTrace
        {
            get { return _stackTrace; }
        }

        public IEnumerable<StepResult> StepResults
        {
            get { return _actionStepResults; }
        }

        public bool HasFailed
        {
            get { return _result is Failed || HasFailedSteps(); }
        }

        public virtual void AddActionStepResults(IEnumerable<StepResult> stepResults)
        {
            foreach (var stepResult in stepResults)
                AddActionStepResult(stepResult);
        }

        public virtual void AddActionStepResult(StepResult actionStepResult)
        {
            _actionStepResults.Add(actionStepResult);
            MergeResult(actionStepResult);
        }

        public void Fail(Exception exception)
        {
            var wrapped = new WrappedException(exception);
            AddToCurrentMessage(wrapped.Message);
            _stackTrace = wrapped.StackTrace;
            _result = new Failed(exception);
        }

        public void Pend(string reason)
        {
            AddToCurrentMessage(reason);

            _result = new Pending(_message);
        }

        protected void MergeResult(StepResult actionStepResult)
        {
            var newResult = actionStepResult.Result;
            if (newResult is Failed)
            {
                _result = newResult;
                Fail(((Failed)newResult).Exception);
            }

            if (newResult is Pending && (_result is Failed == false))
            {
                _result = newResult;
                Pend(newResult.Message);
            }
        }

        private void AddToCurrentMessage(string messageToAdd)
        {
            if (string.IsNullOrEmpty(_message))
            {
                _message = messageToAdd;
            }
            else
            {
                _message += Environment.NewLine + messageToAdd;
            }
        }

        //private string BuildMessage(WrappedException exception)
        //{
        //    return exception.Message;
        //}

        //private string BuildStackTrace(Exception exception)
        //{
        //    return new WrappedException(exception).StackTrace;
        //}

        public bool HasFailedSteps()
        {
            return _actionStepResults.Any(_ => _.Result is Failed);
        }

        public bool HasBackgroundResults()
        {
            return StepResults.Any(_ => _ is BackgroundStepResult);
        }
    }
}