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
            AddAliasFromNarratorAssembly();
        }

        private void AddAliasFromNarratorAssembly()
        {
            var types = GetType().Assembly.GetTypes();
            var actionSteps = from t in types
                              where t.IsSubclassOf(typeof(ActionStepAttribute))
                              select t.Name.Replace("Attribute", "");
            foreach (var alias in actionSteps)
            {
                AddDefaultAlias(new List<string>(), alias);
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

        public IEnumerable<string> GetAliasFor(Type actionStepsAttribute)
        {
            IEnumerable<string> aliasForAttribute = ActionStepAliasConfiguration.GetAliasesForAttribute(actionStepsAttribute);
            string actionStep = TypeAsStringWithoutAttributeAtEnd(actionStepsAttribute);
            AddDefaultAlias(aliasForAttribute, actionStep);
            return Aliases[actionStep];
        }

        public void AddDefaultAlias(IEnumerable<string> aliasList, string actionWord)
        {
            var defaultAliasForGiven = new List<string> { "And" };
            var joinList = GetListFor(actionWord);
            JoinLists(joinList, aliasList);
            if ((actionWord == TypeAsStringWithoutAttributeAtEnd(typeof(GivenAttribute)))
                || (actionWord == TypeAsStringWithoutAttributeAtEnd(typeof(ThenAttribute))))
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

        private string TypeAsStringWithoutAttributeAtEnd(Type type)
        {
            return type.Name.Replace("Attribute", "");
        }

        private IEnumerable<string> JoinListsToNewList(IEnumerable<string> firstList, IEnumerable<string> secondList)
        {
            var list = new List<string>(secondList);
            var notSame = firstList.Except(secondList);
            list.AddRange(notSame);
            return list;
        }

        public IEnumerable<string> GetAliasForTokenString(string tokenString)
        {
            int startIndex = (tokenString.IndexOf(' ') != -1) ? tokenString.IndexOf(' ') : tokenString.Length;
            string firstWord = tokenString.Substring(0, startIndex);
            AddDefaultAlias(ActionStepAliasConfiguration.GetAliasesForAttribute(firstWord), firstWord);

            var tokenAliases = new List<string>();
            string restOfToken = tokenString.Substring(startIndex);
            foreach (var tokenAlias in Aliases[firstWord])
            {
                tokenAliases.Add(tokenAlias + restOfToken);
            }
            return tokenAliases;
        }
    }
}
