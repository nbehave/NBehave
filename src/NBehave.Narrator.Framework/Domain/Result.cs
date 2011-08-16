// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StepResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;

namespace NBehave.Narrator.Framework
{
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
    public class PendingBecauseOfPreviousFailedStep : Result
    {
        public PendingBecauseOfPreviousFailedStep(string message) 
            : base(message)
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