using System;
using System.Collections.Generic;
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
            return action.MatchesFileName(actionStepText.Source);
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