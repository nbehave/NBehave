// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ActionStepAttribute : Attribute
    {
        private static readonly Regex IsCharsAndNumbersOnly = new Regex(@"^(\w|\d|\s)+$");
        private static readonly Regex IsTokenString = new Regex(ActionStepConverterExtensions.TokenRegexPattern);

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
            if (IsRegex(regexOrTokenString))
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

        private static bool IsRegex(string regexOrTokenString)
        {
            if (regexOrTokenString.EndsWith("$"))
            {
                return true;
            }

            if (IsTokenString.IsMatch(regexOrTokenString) 
                || IsCharsAndNumbersOnly.IsMatch(regexOrTokenString))
            {
                return false;
            }
            
            return true;
        }
    }
}