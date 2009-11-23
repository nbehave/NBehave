using System;

namespace NBehave.Narrator.Framework
{
	public class ActionStepResult : Result
	{
		public string ActionStep { get; private set; }
		public Result Result { get; private set; }
		
		public ActionStepResult(string actionStep, Result resultForActionStep)
			: base(resultForActionStep.Message)
		{
			ActionStep = actionStep;
			Result = resultForActionStep;
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
        public string Message { get; protected set; }

        protected Result(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
        	return GetType().Name.Replace(typeof(Result).Name, "").ToLower();
        }
    }
}