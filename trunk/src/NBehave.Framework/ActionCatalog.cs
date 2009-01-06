using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
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

        private object GetActionFromCatalog(string message)
        {
            string keyToUse = message;
            bool actionExist = _actions.ContainsKey(keyToUse);
            if (actionExist == false)
            {
                keyToUse = FindMathingActionKey(message);
                actionExist = (keyToUse != null);
            }
            if (actionExist)
                return _actions[keyToUse];
            return null;
        }

        private string FindMathingActionKey(string message)
        {
            string[] messageWords = SplitWordsToArray(message);
            foreach (string key in _actions.Keys)
            {
                bool allEqual = true;
                int messageWordPos = 0;
                bool isMatch = false;
                string[] actionWords = SplitWordsToArray(key);
                for (int actionWordPos = 0; actionWordPos < actionWords.Length; actionWordPos++)
                {
                    isMatch = false;
                    var word = actionWords[actionWordPos];
                    if (WordIsToken(word))
                    {
                        if (actionWordPos + 1 == actionWords.Length)
                        {
                            isMatch = true;
                        }
                        else
                        {
                            if (messageWords[messageWordPos].EndsWith(actionWords[actionWordPos + 1], StringComparison.InvariantCultureIgnoreCase))
                            {
                                isMatch = true;
                                actionWordPos++;
                            }
                            else if (actionWords[actionWordPos + 1] == messageWords[messageWordPos + 1]) //Dont need to look ahead
                            {
                                isMatch = true;
                            }
                        }
                    }
                    else
                    {
                        isMatch = word.Equals(messageWords[messageWordPos], StringComparison.CurrentCultureIgnoreCase);
                    }
                    allEqual &= isMatch;
                    if (!allEqual)
                        actionWordPos = actionWords.Length;
                    messageWordPos++;
                }
                if (allEqual)
                {
                    return key;
                }
            }
            return null;
        }

        public object[] GetParametersForMessage(string message)
        {
            object action = GetAction(message);
            Type[] args = action.GetType().GetGenericArguments();
            object[] values = new object[args.Length];
            int argNumber = 0;

            string[] messageWords = SplitWordsToArray(message);
            string[] actionWords = SplitWordsToArray(FindMathingActionKey(message));
            int messageWordPos = 0;

            for (int actionWordPos = 0; actionWordPos < actionWords.Length; actionWordPos++)
            {
                var word = actionWords[actionWordPos];
                if (WordIsToken(word))
                {
                    if (word != messageWords[messageWordPos] && actionWordPos + 1 < actionWords.Length)
                    {
                        if (messageWords[messageWordPos].EndsWith(actionWords[actionWordPos + 1], StringComparison.InvariantCultureIgnoreCase))
                        {
                            int charsToRemove = actionWords[actionWordPos + 1].Length;
                            string strParam = messageWords[messageWordPos];
                            strParam = strParam.Remove(strParam.Length - charsToRemove, charsToRemove);
                            values[argNumber] = Convert.ChangeType(strParam, args[argNumber]); //converts string to an instance of args[argNumber]
                            argNumber++;
                            actionWordPos++;
                        }
                        else
                        {
                            string strParam = messageWords[messageWordPos];
                            values[argNumber] = Convert.ChangeType(strParam, args[argNumber]); //converts string to an instance of args[argNumber]
                            argNumber++;
                        }
                    }
                    else
                    {
                        string strParam = messageWords[messageWordPos];
                        values[argNumber] = Convert.ChangeType(strParam, args[argNumber]); //converts string to an instance of args[argNumber]
                        argNumber++;
                    }
                }
                messageWordPos++;
            }
            return values;
        }

        private bool WordIsToken(string word)
        {
            return word.StartsWith("$");
        }

        public object GetAction(string message)
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
            List<string> tokens = new List<string>();

            var matches = Regex.Matches(message, @"\$[a-zA-Z]+");
            foreach (var match in matches)
            {
                tokens.Add(match.ToString());
            }
            return tokens.ToArray();
        }

        private string[] SplitWordsToArray(string message)
        {
            List<string> finalWordList = new List<string>();
            string[] words = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tokens = GetTokensInMessage(message);
            foreach (var word in words)
            {
                if (tokens.Length == 0)
                {
                    finalWordList.Add(word);
                    continue;
                }
                foreach (var token in tokens)
                {
                    if (WordIsToken(word) && word != token)
                    {
                        finalWordList.Add(token);
                        finalWordList.Add(word.Remove(0, token.Length));
                    }
                    else
                        finalWordList.Add(word);
                }
            }
            return finalWordList.ToArray();
        }
    }
}
