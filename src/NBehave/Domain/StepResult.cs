using System;

namespace NBehave
{
    [Serializable]
    public class StepResult : Result
    {
        public StepResult(StringStep stringStep, Result resultForActionStep)
            : base(resultForActionStep.Message)
        {
            StringStep = stringStep;
            Result = resultForActionStep;
        }

        public StringStep StringStep { get; private set; }
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
}