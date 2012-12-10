// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhenAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the WhenAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Attributes
{
    public class WhenAttribute : ActionStepAttribute
    {
        public WhenAttribute()
        {
            Type = "When";            
        }

        public WhenAttribute(string regexOrTokenString)
            : base(regexOrTokenString)
        {
            Type = "When";            
        }
    }
}
