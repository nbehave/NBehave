// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Narrator.Framework.Extensions;

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ActionStepAttribute : Attribute
    {
        protected ActionStepAttribute()
        {
        }

        protected ActionStepAttribute(Regex actionMatch)
        {
            ActionMatch = actionMatch;
            Type = actionMatch.ToString().GetFirstWord();
        }

        protected ActionStepAttribute(string regexOrTokenString)
        {
            if (regexOrTokenString.IsRegex())
            {
                ActionMatch = new Regex(regexOrTokenString);
            }
            else
            {
                ActionMatch = regexOrTokenString.AsRegex();
            }
        }

        public Regex ActionMatch { get; protected set; }

        public string Type { get; protected set; }
    }
}