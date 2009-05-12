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
}