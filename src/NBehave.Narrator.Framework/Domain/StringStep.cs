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

        public StepResult StepResult { get; set; }

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

            return ReferenceEquals(obj, this) || obj.ToString() == ToString();
        }

        public bool Equals(StringStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Step, Step);
        }

        public override int GetHashCode()
        {
            return (Step != null ? Step.GetHashCode() : 0);
        }

        public static bool operator ==(StringStep left, StringStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringStep left, StringStep right)
        {
            return !Equals(left, right);
        }
    }
}