using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionCatalog
    {
        private readonly List<ActionMethodInfo> _actions = new List<ActionMethodInfo>();

        [Obsolete("Use Add(Regex actionMatch, object action)")]
        public void Add(string tokenString, object action, MethodInfo methodInfo)
        {
            if (ActionExists(new ActionStepText(tokenString, "")))
                return;
            var regex = GetRegexForActionKey(tokenString);
            Add(new ActionMethodInfo(regex, action, methodInfo, null));
        }

        public void Add(ActionMethodInfo actionValue)
        {
            _actions.Add(actionValue);
        }

        public bool ActionExists(string text)
        {
            return ActionExists(new ActionStepText(text, ""));
        }

        public bool ActionExists(ActionStepText actionStepText)
        {
            return (FindMatchingAction(actionStepText) != null);
        }

        public object[] GetParametersForActionStepText(ActionStepText actionStepText)
        {
            ActionMethodInfo action = GetAction(actionStepText);
            List<string> paramNames = GetParameterNames(action);
            Match match = action.ActionStepMatcher.Match(actionStepText.Step);
            Func<int, string> getValues = i => match.Groups[paramNames[i]].Value;

            return GetParametersForActionStepText(action, paramNames, getValues);
        }

        public object[] GetParametersForActionStepText(ActionStepText actionStepText, Row row)
        {
            ActionMethodInfo action = GetAction(actionStepText);
            List<string> paramNames = action.ParameterInfo.Select(a => a.Name).ToList();
            Func<int, string> getValues = i => row.ColumnValues[paramNames[i]];

            return GetParametersForActionStepText(action, paramNames, getValues);
        }

        private object[] GetParametersForActionStepText(ActionMethodInfo action, ICollection<string> paramNames, Func<int, string> getValue)
        {
            ParameterInfo[] args = action.ParameterInfo;
            var values = new object[args.Length];

            for (int argNumber = 0; argNumber < paramNames.Count; argNumber++)
            {
                string strParam = getValue(argNumber);
                values[argNumber] = ChangeParameterType(strParam, args[argNumber]);
            }

            return values;
        }

        private object ChangeParameterType(string strParam, ParameterInfo paramType)
        {
            if (paramType.ParameterType.IsArray)
                return CreateArray(strParam, paramType.ParameterType);
            return Convert.ChangeType(strParam, paramType.ParameterType);
        }

        private object CreateArray(string strParam, Type arrayOfType)
        {
            var strParamAsArray = strParam.Replace(Environment.NewLine, "\n").Split(new[] { ',' });
            for (int i = 0; i < strParamAsArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strParamAsArray[i]) == false)
                    strParamAsArray[i] = strParamAsArray[i].Trim();
            }
            while (string.IsNullOrEmpty(strParamAsArray.Last()))
                strParamAsArray = strParamAsArray.Take(strParamAsArray.Length - 1).ToArray();
            var typedArray = Activator.CreateInstance(arrayOfType, strParamAsArray.Length);
            var typeInList = arrayOfType.GetElementType();
            var method = GetType().GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var types = new[] { typeInList };
            var genMethod = method.MakeGenericMethod(types);
            for (int i = 0; i < strParamAsArray.Length; i++)
            {
                object value = Convert.ChangeType(strParamAsArray[i], typeInList);
                genMethod.Invoke(this, new[] { typedArray, i, value });
            }
            return typedArray;
        }

        //This method is called with reflection by the CreateArray method
        private void SetValue<T>(T[] array, int index, T value)
        {
            array[index] = value;
        }


        public ActionMethodInfo GetAction(ActionStepText message)
        {
            return FindMatchingAction(message);
        }

        public string BuildMessage(string message, object[] parameters)
        {
            string resultString = message;
            string[] tokens = GetTokensInMessage(message);
            if (tokens.Length > 0 && tokens.Length != parameters.Length)
                throw new ArgumentException(string.Format("message has {0} tokens and there are {1} parameters", tokens.Length,
                                                          parameters.Length));
            for (int i = 0; i < tokens.Length; i++)
            {
                resultString = resultString.Replace(tokens[i], parameters[i].ToString());
            }

            return resultString;
        }

        private ActionMethodInfo FindMatchingAction(ActionStepText actionStepText)
        {
            string message = actionStepText.Step;
            ActionMethodInfo matchedAction = null;
            int lengthOfMatch = -1;
            foreach (ActionMethodInfo action in _actions)
            {
                Regex regex = action.ActionStepMatcher;
                var match = regex.Match(message);
                if (match.Success)
                {
                    if (MatchesFileName(action, actionStepText)
                        && match.Value.Length > lengthOfMatch)
                    {
                        lengthOfMatch = match.Value.Length;
                        matchedAction = action;
                    }
                }
            }
            return matchedAction;
        }

        private bool MatchesFileName(ActionMethodInfo action, ActionStepText actionStepText)
        {
            return action.MatchesFileName(actionStepText.FromFile);
        }

        private List<string> GetParameterNames(ActionMethodInfo actionValue)
        {
            return actionValue.GetParameterNames();
        }

        private string[] GetTokensInMessage(string message)
        {
            var tokens = new List<string>();

            var matches = Regex.Matches(message, ActionStepConverterExtensions.TokenRegexPattern);
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