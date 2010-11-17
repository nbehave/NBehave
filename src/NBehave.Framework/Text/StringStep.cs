// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringStep.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public class StringStep : ActionStepText
    {
        public StringStep(string step, string fromFile, IStringStepRunner stringStepRunner) : base(step, fromFile)
        {
            StringStepRunner = stringStepRunner;
        }

        public ActionStepResult StepResult { get; protected set; }

        protected IStringStepRunner StringStepRunner { get; private set; }

        public override string ToString()
        {
            return Step;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj as StringStep == null)
            {
                return false;
            }

            return (obj == this) || obj.ToString() == ToString();
        }

        public virtual void Run()
        {
            StepResult = StringStepRunner.Run(this);
        }
    }
}