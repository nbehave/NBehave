using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionValue
    {
        public object Action { get; set; }
        public string Key { get; set; }
    }

    public class ActionCatalog
    {
        public const char TokenPrefix = '$';

        private readonly Dictionary<string, object> _actions = new Dictionary<string, object>();

        public void Add(string message, object action)
        {
            if (ActionExists(message))
                return;

            _actions.Add(message, action);
        }

        public bool ActionExists(string message)
        {
            bool actionExist = _actions.ContainsKey(message);
            if (actionExist == false)
                actionExist = (FindMathingActionKey(message) != null);
            return actionExist;
        }

        public bool CatalogedActionIsTokenized(string message)
        {
            string catalogedAction = FindMathingActionKey(message);
            return (catalogedAction != null && catalogedAction.IndexOf(TokenPrefix) > -1);
        }

        private ActionValue GetActionFromCatalog(string message)
        {
            string keyToUse = message;
            bool actionExist = _actions.ContainsKey(keyToUse);
            if (actionExist == false)
            {
                keyToUse = FindMathingActionKey(message);
                actionExist = (keyToUse != null);
            }
            if (actionExist)
                return new ActionValue { Key = keyToUse, Action = _actions[keyToUse] };
            return null;
        }

        private string FindMathingActionKey(string message)
        {
            foreach (string key in _actions.Keys)
            {
                Regex regex = GetRegexForActionKey(key);
                bool isMatch = regex.IsMatch(message);
                if (isMatch)
                    return key;
            }

            return null;
        }

        private IEnumerable<string> GetActionKeys(object action)
        {
            return from r in _actions
                   where r.Value.Equals(action)
                   select r.Key;
        }

        private List<string> GetParamTokens(ActionValue actionValue)
        {
            var tokenNames = new List<string>();
            string actionKey = actionValue.Key;
            string[] words = actionKey.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (WordIsToken(word))
                {
                    tokenNames.Add(GetValidRegexGroupName(word));
                }
            }
            return tokenNames;
        }

        public object[] GetParametersForMessage(string message)
        {
            ActionValue action = GetAction(message);
            IEnumerable<Regex> actionRegex = GetRegexForAction(action.Action);
            List<string> paramNames = GetParamTokens(action);
            Type[] args = action.Action.GetType().GetGenericArguments();
            var values = new object[args.Length];

            Match match = GetOneMatch(actionRegex, message);
            for (int argNumber = 0; argNumber < paramNames.Count(); argNumber++)
            {
                var strParam = match.Groups[paramNames[argNumber]].Value;
                values[argNumber] = Convert.ChangeType(strParam, args[argNumber]); //converts string to an instance of args[argNumber]
            }
            return values;
        }

        private Match GetOneMatch(IEnumerable<Regex> actionRegexes, string message)
        {
            foreach (var actionRegex in actionRegexes)
            {
                Match match = actionRegex.Match(message);
                if (match.Success)
                    return match;
            }
            return null;
        }

        private bool WordIsToken(string word)
        {
            return word.StartsWith("$");
        }

        public ActionValue GetAction(string message)
        {
            return GetActionFromCatalog(message);
        }

        public string BuildMessage(string message, object[] parameters)
        {
            string resultString = message;
            string[] tokens = GetTokensInMessage(message);
            if (tokens.Length > 0 && tokens.Length != parameters.Length)
                throw new ArgumentException(string.Format("message has {0} tokens and there are {1} parameters", tokens.Length, parameters.Length));
            for (int i = 0; i < tokens.Length; i++)
            {
                resultString = resultString.Replace(tokens[i], parameters[i].ToString());
            }

            return resultString;
        }

        private string[] GetTokensInMessage(string message)
        {
            var tokens = new List<string>();

            var matches = Regex.Matches(message, @"\$[a-zA-Z]+");
            foreach (var match in matches)
            {
                tokens.Add(match.ToString());
            }
            return tokens.ToArray();
        }

        private IEnumerable<Regex> GetRegexForAction(object action)
        {
            IEnumerable<string> actionKeys = GetActionKeys(action);
            var regexes = new List<Regex>();
            foreach (var actionKey in actionKeys)
                regexes.Add(GetRegexForActionKey(actionKey));
            return regexes;
        }

        private Regex GetRegexForActionKey(string actionKey)
        {
            string[] words = actionKey.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string regex = "^";
            foreach (var word in words)
            {
                if (WordIsToken(word))
                {
                    var groupName = GetValidRegexGroupName(word);
                    var stuffAtEnd = RemoveTokenPrefix(word).Replace(groupName, string.Empty);
                    regex += string.Format(@"(?<{0}>(\-?\w+\s*)+){1}\s+", groupName, stuffAtEnd);
                }
                else
                    regex += string.Format(@"{0}\s+", word);
            }
            if (regex.EndsWith(@"\s+"))
                regex = regex.Substring(0, regex.Length - 1) + "*";
            regex += "$";
            return new Regex(regex, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        }

        private string GetValidRegexGroupName(string word)
        {
            var regex = new Regex(@"\w+");
            return regex.Match(word).Value;
        }

        private string RemoveTokenPrefix(string word)
        {
            return word.Substring(TokenPrefix.ToString().Length);
        }
    }
}
