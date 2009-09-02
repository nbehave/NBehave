using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionCatalog
    {
        public const char TokenPrefix = '$';

        private readonly List<ActionValue> _actions = new List<ActionValue>();

        [Obsolete("Use Add(Regex actionMatch, object action)")]
        public void Add(string tokenString, object action, ParameterInfo[] parameters)
        {
            if (ActionExists(tokenString))
                return;
            var regex = GetRegexForActionKey(tokenString);
            Add(new ActionValue(regex, action, parameters));
        }

        public void Add(ActionValue actionValue)
        {
            _actions.Add(actionValue);
        }

        public bool ActionExists(string message)
        {
            return (FindMathingAction(message) != null);
        }

        public string BuildFormatString(string message, ICollection<object> args)
        {
            if ((message.IndexOf(TokenPrefix) == -1))
            {
                if (args.Count == 0)
                    return "{0} {1}";
                if (args.Count == 1)
                    return "{0} {1}: {2}";
                string formatString = "{0} {1}: (";
                for (int i = 0; i < args.Count; i++)
                    formatString += "{" + (i + 2) + "}, ";
                return formatString.Remove(formatString.Length - 2) + ")";
            }
            return "{0} {1}";
        }

        public object[] GetParametersForMessage(string message)
        {
            ActionValue action = GetAction(message);
            List<string> paramNames = GetParameterNames(action);
            //Type[] args = action.Action.GetType().GetGenericArguments();
            ParameterInfo[] args = action.ParameterInfo;
            var values = new object[args.Length];

            Match match = action.ActionStepMatcher.Match(message);
            for (int argNumber = 0; argNumber < paramNames.Count(); argNumber++)
            {
                var strParam = match.Groups[paramNames[argNumber]].Value;
                if (args[argNumber].ParameterType.IsArray)
                {
                    var strParamAsArray = strParam.Replace(Environment.NewLine, "\n")
                        .Split(new[] { '\n' });
                    while (string.IsNullOrEmpty(strParamAsArray.Last()))
                        strParamAsArray = strParamAsArray.Take(strParamAsArray.Length - 1).ToArray();
                    values[argNumber] = Convert.ChangeType(strParamAsArray, args[argNumber].ParameterType); //converts string to an instance of args[argNumber]
                }
                else
                    values[argNumber] = Convert.ChangeType(strParam, args[argNumber].ParameterType); //converts string to an instance of args[argNumber]
            }
            return values;
        }

        public ActionValue GetAction(string message)
        {
            return FindMathingAction(message);
        }

        public string BuildMessage(string message, object[] parameters)
        {
            string resultString = message;
            string[] tokens = GetTokensInMessage(message);
            if (tokens.Length > 0 && tokens.Length != parameters.Length)
                throw new ArgumentException(string.Format("message has {0} tokens and there are {1} parameters", tokens.Length, parameters.Length));
            for (int i = 0; i < tokens.Length; i++)
            {
                resultString = resultString.Replace(tokens[i], parameters[i].ToString());
            }

            return resultString;
        }

        private ActionValue FindMathingAction(string message)
        {
            foreach (var action in _actions)
            {
                Regex regex = action.ActionStepMatcher;
                bool isMatch = regex.IsMatch(message);
                if (isMatch)
                    return action;
            }
            return null;
        }

        private List<string> GetParameterNames(ActionMatch actionValue)
        {
            return actionValue.GetParameterNames();
        }

        private string[] GetTokensInMessage(string message)
        {
            var tokens = new List<string>();

            var matches = Regex.Matches(message, @"\$[a-zA-Z]+");
            foreach (var match in matches)
            {
                tokens.Add(match.ToString());
            }
            return tokens.ToArray();
        }

        private Regex GetRegexForActionKey(string actionKey)
        {
            return actionKey.AsRegex();
        }
    }
}
