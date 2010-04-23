using System;

namespace NBehave.Narrator.Framework
{
    public class StringStep : ActionStepText
    {
        protected IStringStepRunner StringStepRunner { get; private set; }

        public StringStep(string step, string fromFile, IStringStepRunner stringStepRunner)
            : base(step, fromFile)
        {
            StringStepRunner = stringStepRunner;
        }

        public override string ToString()
        {
            return Step;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj as StringStep == null)
                return false;

            return ((obj == this) || obj.ToString() == ToString());
        }

        public virtual void Run()
        {
            StepResult = StringStepRunner.Run(this);
        }

        public ActionStepResult StepResult { get; protected set; }
    }
}