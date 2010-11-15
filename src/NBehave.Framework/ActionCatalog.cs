using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionCatalog
    {
        private readonly List<ActionMethodInfo> _actions = new List<ActionMethodInfo>();

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
            var resultString = message;
            var tokens = GetTokensInMessage(message);
            if (tokens.Length > 0 && tokens.Length != parameters.Length)
                throw new ArgumentException(string.Format("message has {0} tokens and there are {1} parameters", tokens.Length,
                                                          parameters.Length));
            for (var i = 0; i < tokens.Length; i++)
            {
                resultString = resultString.Replace(tokens[i], parameters[i].ToString());
            }

            return resultString;
        }

        private ActionMethodInfo FindMatchingAction(ActionStepText actionStepText)
        {
            var message = actionStepText.Step;
            ActionMethodInfo matchedAction = null;
            var lengthOfMatch = -1;
            foreach (var action in _actions)
            {
                var regex = action.ActionStepMatcher;
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
    }
}