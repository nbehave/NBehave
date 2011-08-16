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
        
        public bool ActionExists(StringStep stringStep)
        {
            return FindMatchingAction(stringStep) != null;
        }

        public ActionMethodInfo GetAction(StringStep message)
        {
            return FindMatchingAction(message);
        }

        private ActionMethodInfo FindMatchingAction(StringStep stringStep)
        {
            ActionMethodInfo matchedAction = null;
            var lengthOfMatch = -1;
            foreach (var action in _actions)
            {
                var regex = action.ActionStepMatcher;
                var match = regex.Match(stringStep.MatchableStep);
                if (match.Success)
                {
                    if (MatchesFileName(action, stringStep)
                        && match.Value.Length > lengthOfMatch)
                    {
                        lengthOfMatch = match.Value.Length;
                        matchedAction = action;
                    }
                }
            }

            return matchedAction;
        }

        private bool MatchesFileName(ActionMethodInfo action, StringStep stringStep)
        {
            return action.MatchesFileName(stringStep.Source);
        }
    }
}