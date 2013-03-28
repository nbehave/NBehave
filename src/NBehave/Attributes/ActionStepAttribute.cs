using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using NBehave.Extensions;

namespace NBehave
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [MeansImplicitUse]
    public abstract class ActionStepAttribute : Attribute
    {
        protected ActionStepAttribute()
        {
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

        public void BuildActionMatchFromMethodInfo(MethodInfo method, ActionStepAttribute attrib)
        {
            var tokenString = BuildTokenString(method);
            var chars = new[] {' ', '$', '['};
            var specialMethod = chars.Any(c => method.Name.Contains(c));
            var tokenStringWithoutFirstWord = specialMethod ? tokenString : tokenString.RemoveFirstWord();
            ActionMatch = tokenStringWithoutFirstWord.AsRegex();
            Type = specialMethod ? attrib.Type : tokenString.GetFirstWord();
        }

        private string BuildTokenString(MethodInfo method)
        {
            var words = method.Name.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                var paramName = param.Name;

                var idx = words.LastIndexOf(paramName.ToUpper());
                if (idx >= 0)
                    words[idx] = "$" + paramName;
                else
                {
                    idx = words.LastIndexOf(paramName);
                    if (idx >= 0)
                        words[idx] = "$" + paramName;
                }
            }

            return string.Join(" ", words.ToArray());
        }
    }
}
