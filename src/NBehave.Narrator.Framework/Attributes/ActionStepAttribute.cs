// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
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

        public Regex ActionMatch { get; private set; }

        public string Type { get; protected set; }

        public void BuildActionMatchFromMethodInfo(MethodInfo method)
        {
            var tokenString = BuildTokenString(method);
            var tokenStringWithoutFirstWord = tokenString.RemoveFirstWord();
            ActionMatch = tokenStringWithoutFirstWord.AsRegex();
            Type = tokenString.GetFirstWord();
        }

        private string BuildTokenString(MethodInfo method)
        {
            var methodName = method.Name.Replace('_', ' ');
            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                var paramName = param.Name;
                var pos = methodName.IndexOf(paramName);
                if (pos > 0)
                {
                    methodName = methodName.Substring(0, pos) + "$" + methodName.Substring(pos);
                }
            }

            return methodName;
        }
    }
}