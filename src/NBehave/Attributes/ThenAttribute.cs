// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThenAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ThenAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Attributes
{
    public class ThenAttribute : ActionStepAttribute
    {
        public ThenAttribute()
        {
            Type = "Then";
        }

        public ThenAttribute(string regexOrTokenString) : base(regexOrTokenString)
        {
            Type = "Then";            
        }
    }
}