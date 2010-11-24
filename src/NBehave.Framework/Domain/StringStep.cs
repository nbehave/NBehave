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
        public StringStep(string step, string fromFile) : base(step, fromFile)
        {
        }

        public ActionStepResult StepResult { get; set; }

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
    }
}