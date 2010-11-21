// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionCatalog.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionCatalog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    public class ActionCatalog
    {
        private readonly List<ActionMethodInfo> _actions = new List<ActionMethodInfo>();

        public void Add(ActionMethodInfo actionValue)
        {
            _actions.Add(actionValue);
        }

        public bool ActionExists(string text)
        {
            return ActionExists(new ActionStepText(text, string.Empty));
        }

        public bool ActionExists(ActionStepText actionStepText)
        {
            return FindMatchingAction(actionStepText) != null;
        }

        public ActionMethodInfo GetAction(ActionStepText message)
        {
            return FindMatchingAction(message);
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
    }
}