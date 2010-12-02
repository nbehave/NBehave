using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class ScenarioResult : MarshalByRefObject
    {
        private Result _result = new Passed();
        private string _message;
        private string _stackTrace;
        private readonly List<ActionStepResult> _actionStepResults;

        public ScenarioResult(Feature feature, string scenarioTitle)
        {
            FeatureTitle = feature.Title;
            ScenarioTitle = scenarioTitle;
            _actionStepResults = new List<ActionStepResult>();
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

        public virtual void AddActionStepResult(ActionStepResult actionStepResult)
        {
            _actionStepResults.Add(actionStepResult);
            MergeResult(actionStepResult);
        }

        protected void MergeResult(ActionStepResult actionStepResult)
        {
            Result newResult = actionStepResult.Result;
            if (newResult.GetType() == typeof(Failed))
            {
                _result = newResult;
                Fail(((Failed)newResult).Exception);
            }
            if (newResult.GetType() == typeof(Pending) && _result.GetType() != typeof(Failed))
            {
                _result = newResult;
                Pend(newResult.Message, actionStepResult.StringStep);
            }
        }

        public IEnumerable<ActionStepResult> ActionStepResults
        {
            get { return _actionStepResults; }
        }

        public void Fail(Exception exception)
        {
            AddToCurrentMessage(BuildMessage(exception));
            _stackTrace = BuildStackTrace(exception);
            _result = new Failed(exception);
        }

        public void Pend(string reason, string step)
        {
            AddToCurrentMessage(reason);

            _result = new Pending(_message);
        }

        private void AddToCurrentMessage(string messageToAdd)
        {
            if (string.IsNullOrEmpty(_message))
                _message = messageToAdd;
            else
                _message += Environment.NewLine + messageToAdd;
        }

        private string BuildMessage(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} : {1}", exception.GetType(), exception.Message);

            Exception inner = exception.InnerException;
            while (inner != null)
            {
                sb.AppendLine();
                sb.AppendFormat("  ----> {0} : {1}", inner.GetType(), inner.Message);
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        private string BuildStackTrace(Exception exception)
        {
            var builder = new StringBuilder(exception.StackTrace);

            Exception inner = exception.InnerException;
            while (inner != null)
            {
                builder.AppendLine();
                builder.Append("--");
                builder.Append(inner.GetType().Name);
                builder.AppendLine();
                builder.Append(inner.StackTrace);
                inner = inner.InnerException;
            }
            return builder.ToString();
        }
    }
}