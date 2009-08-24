using System;

namespace NBehave.Narrator.Framework
{
    public class Passed : Result
    {
        public Passed()
            : base(string.Empty)
        {
        }

        public override string ResultIndicator
        {
            get { return "."; }
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

        public override string ResultIndicator
        {
            get { return "F"; }
        }
    }

    public class Pending : Result
    {
        public Pending(string pendingReason)
            : base(pendingReason)
        {
        }

        public override string ResultIndicator
        {
            get { return "P"; }
        }
    }

    public abstract class Result
    {
        public string Message { get; protected set; }
        public abstract string ResultIndicator { get; }

        protected Result(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return GetType().Name.Replace(typeof(Result).Name, "");
        }
    }
}