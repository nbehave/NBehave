using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    /// <summary>
    /// Use Given, When or Then Attribute
    /// </summary>
    public abstract class ActionStepAttribute : Attribute
    {
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

        private static readonly Regex _isTokenString = new Regex(@"\$\w+");

        private bool IsRegex(string regexOrTokenString)
        {
            return _isTokenString.IsMatch(regexOrTokenString) == false;
        }

        protected ActionStepAttribute(Regex actionMatch)
        {
            ActionMatch = actionMatch;
            Type = actionMatch.ToString().GetFirstWord();
        }
    }
}