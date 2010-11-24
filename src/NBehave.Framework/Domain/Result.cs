// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;

    public class ActionStepResult : Result
    {
        public ActionStepResult(string stringStep, Result resultForActionStep)
            : base(resultForActionStep.Message)
        {
            StringStep = stringStep;
            Result = resultForActionStep;
        }

        public string StringStep { get; private set; }

        public Result Result { get; private set; }

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

    public class Passed : Result
    {
        public Passed()
            : base(string.Empty)
        {
        }
    }

    public class Failed : Result
    {
        public Exception Exception { get; private set; }
        public Failed(Exception exception)
            : base(exception.ToString())
        {
            Exception = exception;
        }
    }

    public class Pending : Result
    {
        public Pending(string pendingReason)
            : base(pendingReason)
        {
        }
    }

    public abstract class Result
    {
        protected Result(string message)
        {
            Message = message;
        }

        public string Message { get; protected set; }

        public override string ToString()
        {
            return GetType().Name.Replace(typeof(Result).Name, string.Empty).ToLower();
        }
    }
}