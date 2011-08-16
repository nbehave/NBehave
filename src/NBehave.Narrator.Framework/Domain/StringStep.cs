// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringStep.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class StringStep : ActionStepText
    {
        public StringStep(string step, string fromFile)
            : base(step, fromFile)
        { }

        public StepResult StepResult { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringStep);
        }

        public bool Equals(StringStep other)
        {
            if (other == null)
                return false;
            return (ReferenceEquals(this, other)) || (other.MatchableStep == MatchableStep && other.Source == Source);
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