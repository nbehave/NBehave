// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionCatalog.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionCatalog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public enum MethodParametersType
    {
        TypedStep,
        UntypedStep,
        UntypedListStep,
        TypedListStep
    }

    public class ActionCatalog
    {
        private readonly List<ActionMethodInfo> _actions = new List<ActionMethodInfo>();
        private readonly List<MethodParametersType> allActionTypes = Enum.GetValues(typeof(MethodParametersType)).Cast<MethodParametersType>().ToList();

        public void Add(ActionMethodInfo actionValue)
        {
            _actions.Add(actionValue);
        }

        public bool ActionExists(StringStep stringStep)
        {
            return FindMatchingAction(stringStep, allActionTypes) != null;
        }

        public ActionMethodInfo GetAction(StringStep message)
        {
            return FindMatchingAction(message, null);
        }

        private ActionMethodInfo FindMatchingAction(StringStep stringStep, List<MethodParametersType> sortMethodAfter)
        {
            sortMethodAfter = sortMethodAfter ?? stringStep.MatchableStepTypes.ToList();
            var ms = _actions
                .Select(_ => new { Action = _, Match = _.ActionStepMatcher.Match(stringStep.MatchableStep) })
                .Where(_ => _.Match.Success && MatchesFileName(_.Action, stringStep))
                .Select(_ => new
                        {
                            action = _.Action,
                            _.Match.Length,
                            Prio = sortMethodAfter.Contains(_.Action.MethodParametersType) ? sortMethodAfter.IndexOf(_.Action.MethodParametersType) : Int32.MaxValue
                        })
                .OrderByDescending(_ => _.Length).ThenBy(_ => _.Prio);
            return ms.Select(_ => _.action).FirstOrDefault();
        }

        private bool MatchesFileName(ActionMethodInfo action, StringStep stringStep)
        {
            return action.MatchesFileName(stringStep.Source);
        }
    }
}