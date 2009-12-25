using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace NBehave.Narrator.Framework
{
    public static class ActionStepAliasConfiguration
    {
        private static readonly NameValueCollection _config = ConfigurationManager.GetSection("NBehave") as NameValueCollection;

        private static IEnumerable<string> GetValue(string configKey)
        {
            if (_config == null)
                return new List<string>();
            string value = _config[configKey] ?? string.Empty;
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> GetAliasesForActionType(string actionType)
        {
            if (string.IsNullOrEmpty(actionType))
                return new List<string>();
            return GetValue(string.Format("Alias.{0}", actionType));
        }

        public static IEnumerable<string> ActionSteps
        {
            get { return GetValue("ActionStep"); }
        }
    }
}