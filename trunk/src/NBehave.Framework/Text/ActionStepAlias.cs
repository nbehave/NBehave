using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class ActionStepAlias
    {
        private readonly Dictionary<string, List<string>> _aliases = new Dictionary<string, List<string>>();

        public ActionStepAlias()
        {
            AddDefaultAlias();
        }

        private IEnumerable<string> DefaultActionSteps
        {
            get
            {
                var defaultActionSteps = new List<string>();
                defaultActionSteps.AddRange(ActionStep.StorySteps);
                defaultActionSteps.AddRange(ActionStep.ScenarioSteps);
                return defaultActionSteps;
            }
        }

        private void AddDefaultAlias()
        {
            var actionSteps = ActionStepAliasConfiguration.ActionSteps;
            if (actionSteps == null || actionSteps.Count() == 0)
                actionSteps = DefaultActionSteps;
            foreach (var actionStep in actionSteps)
            {
                AddDefaultAlias(new List<string>(), actionStep);
            }
        }

        public Dictionary<string, List<string>> AliasesForAllActionWords
        {
            get
            {
                return Aliases;
            }
        }

        public Dictionary<string, List<string>> Aliases
        {
            get { return _aliases; }
        }

        public void AddDefaultAlias(IEnumerable<string> aliasList, string actionWord)
        {
            var defaultAliasForGiven = new List<string> { "And" };
            var joinList = GetListFor(actionWord);
            JoinLists(joinList, aliasList);
            if ((actionWord.Equals("Given", StringComparison.CurrentCultureIgnoreCase))
                || (actionWord.Equals("Then", StringComparison.CurrentCultureIgnoreCase)))
            {
                JoinLists(joinList, defaultAliasForGiven);
            }
        }

        private void JoinLists(List<string> destinationList, IEnumerable<string> list)
        {
            IEnumerable<string> tmpList = JoinListsToNewList(destinationList, list);
            destinationList.Clear();
            destinationList.AddRange(tmpList);
        }

        private List<string> GetListFor(string actionWord)
        {
            List<string> list;
            if (Aliases.TryGetValue(actionWord, out list) == false)
            {
                list = new List<string>();
                Aliases.Add(actionWord, list);
            }
            return list;
        }

        private IEnumerable<string> JoinListsToNewList(IEnumerable<string> firstList, IEnumerable<string> secondList)
        {
            var list = new List<string>(secondList);
            var notSame = firstList.Except(secondList);
            list.AddRange(notSame);
            return list;
        }

        public IEnumerable<string> GetAliasesForActionType(string actionType, string tokenString)
        {
            AddDefaultAlias(ActionStepAliasConfiguration.GetAliasesForActionType(actionType), actionType);

            var tokenAliases = new List<string>();
            foreach (var tokenAlias in Aliases[actionType])
            {
                string newTokenString = tokenString.ReplaceFirst(actionType, tokenAlias);
                tokenAliases.Add(newTokenString);
            }
            return tokenAliases;
        }

        public IEnumerable<string> GetAliasFor(string actionType)
        {
            IEnumerable<string> aliasForAttribute = ActionStepAliasConfiguration.GetAliasesForActionType(actionType);
            AddDefaultAlias(aliasForAttribute, actionType);
            return Aliases[actionType];
        }
    }
}
