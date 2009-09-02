using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    [Obsolete("Kill this type and use ActionMethodInfo instead")]
    public class ActionValue : ActionMatch
    {
        public object Action { get; set; }
        public ParameterInfo[] ParameterInfo { get; private set; }

        public ActionValue(Regex actionStepMatcher, object action, ParameterInfo[] parameterInfo)
        {
            Action = action;
            ParameterInfo = parameterInfo;
            ActionStepMatcher = actionStepMatcher;
        }
    }
}