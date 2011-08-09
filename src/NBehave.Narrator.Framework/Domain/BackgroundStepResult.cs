using System;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class BackgroundStepResult : StepResult
    {
        public BackgroundStepResult(string backgroundTitle, StepResult result) 
            : base(result.StringStep, result.Result)
        {
            BackgroundTitle = backgroundTitle;
        }

        public string BackgroundTitle { get; private set; }
    }
}