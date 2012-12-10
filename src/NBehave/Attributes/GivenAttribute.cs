// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GivenAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the GivenAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public class GivenAttribute : ActionStepAttribute
    {
        public GivenAttribute()
        {
            Type = "Given";            
        }

        public GivenAttribute(string regexOrTokenString)
            : base(regexOrTokenString)
        {
            Type = "Given";            
        }
    }
}