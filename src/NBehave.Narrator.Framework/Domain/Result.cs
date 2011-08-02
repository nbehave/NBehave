// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StepResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;

    [Serializable]
    public class StepResult : Result
    {
        public StepResult(string stringStep, Result resultForActionStep)
            : base(resultForActionStep.Message)
        {
            StringStep = stringStep;
            Result = resultForActionStep;
        }

        private string _stringStep;
        public string StringStep
        {
            get { return _stringStep; }
            private set { _stringStep = value; }
        }

        private Result _result;
        public Result Result
        {
            get { return _result; }
            private set { _result = value; }
        }

        public void MergeResult(Result stepResult)
        {
            if (stepResult is Passed)
            {
                return;
            }

            if (stepResult is Pending && Result is Passed)
            {
                Result = stepResult;
                Message = stepResult.Message;
            }

            if (stepResult is Failed && (Result is Passed || Result is Pending))
            {
                Result = stepResult;
                Message = stepResult.Message;
            }

            if (Result == null)
            {
                Result = stepResult;
                Message = stepResult.Message;
            }
        }

        public override string ToString()
        {
            return Result.ToString();
        }
    }

    [Serializable]
    public class Passed : Result
    {
        public Passed()
            : base(string.Empty)
        {
        }
    }

    [Serializable]
    public class Failed : Result
    {
        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
            private set { _exception = value; }
        }

        public Failed(Exception exception)
            : base(exception.ToString())
        {
            Exception = exception;
        }
    }

    [Serializable]
    public class Pending : Result
    {
        public Pending(string pendingReason)
            : base(pendingReason)
        {
        }
    }

    [Serializable]
    public abstract class Result
    {
        protected Result(string message)
        {
            Message = message;
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            protected set { _message = value; }
        }

        public override string ToString()
        {
            return GetType().Name.Replace(typeof(Result).Name, string.Empty).ToLower();
        }
    }
}