using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace NBehave.Narrator.Framework
{
    public static class ActionStepAliasConfiguration
    {
        private static readonly NameValueCollection _config = ConfigurationManager.GetSection("NBehave") as NameValueCollection;

        public static IEnumerable<string> GetAliasesForAttribute(Type actionStepAttribute)
        {
            return GetAliasesForAttribute(actionStepAttribute.Name.Replace("Attribute", ""));
        }

        public static IEnumerable<string> GetAliasesForAttribute(string actionStep)
        {
            string value = _config[string.Format("Alias.{0}", actionStep)] ?? string.Empty;
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public class ActionStepAlias
    {
        public IEnumerable<string> GetAliasFor(Type actionStepsAttribute)
        {
            IEnumerable<string> aliasForAttribute = ActionStepAliasConfiguration.GetAliasesForAttribute(actionStepsAttribute);

            return AddDefaultAlias(aliasForAttribute, TypeAsStringWithoutAttributeAtEnd(actionStepsAttribute));
        }

        private IEnumerable<string> AddDefaultAlias(IEnumerable<string> aliasList, string magicWord)
        {
            var defaultAliasForGiven = new List<string> { "And" };


            if ((magicWord == TypeAsStringWithoutAttributeAtEnd(typeof(GivenAttribute)))
                || (magicWord == TypeAsStringWithoutAttributeAtEnd(typeof(ThenAttribute))))
                return JoinLists(defaultAliasForGiven, aliasList);
            return aliasList;
        }

        private string TypeAsStringWithoutAttributeAtEnd(Type type)
        {
            return type.Name.Replace("Attribute", "");
        }

        private static IEnumerable<string> JoinLists(IEnumerable<string> defaultAliasForGiven, IEnumerable<string> aliasForAttribute)
        {
            var list = new List<string>(aliasForAttribute);
            list.AddRange(defaultAliasForGiven);
            return list;
        }

        public IEnumerable<string> GetAliasForTokenString(string tokenString)
        {
            int startIndex = tokenString.IndexOf(' ') != -1 ? tokenString.IndexOf(' ') : tokenString.Length;
            string firstWord = tokenString.Substring(0, startIndex);
            IEnumerable<string> aliasForAttribute = AddDefaultAlias(ActionStepAliasConfiguration.GetAliasesForAttribute(firstWord), firstWord);
            var tokenAliases = new List<string>();
            string restOfToken = tokenString.Substring(startIndex);
            foreach (var tokenAlias in aliasForAttribute)
            {
                tokenAliases.Add(tokenAlias + restOfToken);
            }
            return tokenAliases;
        }
    }
}
