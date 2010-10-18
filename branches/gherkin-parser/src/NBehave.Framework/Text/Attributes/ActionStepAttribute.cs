using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    /// <summary>
    /// Use Given, When or Then Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ActionStepAttribute : Attribute
    {
        private static readonly Regex IsCharsAndNumbersOnly = new Regex(@"^(\w|\d|\s)+$"); //new Regex(@"\$\w+");
        private static readonly Regex IsTokenString = new Regex(ActionStepConverterExtensions.TokenRegexPattern); //new Regex(@"\$\w+");
        
        public Regex ActionMatch { get; protected set; }
        public string Type { get; protected set; }

        protected ActionStepAttribute()
        { }

        protected ActionStepAttribute(string regexOrTokenString)
        {
            if (IsRegex(regexOrTokenString))
                ActionMatch = new Regex(regexOrTokenString);
            else
                ActionMatch = regexOrTokenString.AsRegex();
        }

        private bool IsRegex(string regexOrTokenString)
        {
            if (regexOrTokenString.EndsWith("$"))
                return true;
            if (IsTokenString.IsMatch(regexOrTokenString) 
                || IsCharsAndNumbersOnly.IsMatch(regexOrTokenString))
                return false;
            return true;
        }

        protected ActionStepAttribute(Regex actionMatch)
        {
            ActionMatch = actionMatch;
            Type = actionMatch.ToString().GetFirstWord();
        }
    }
}