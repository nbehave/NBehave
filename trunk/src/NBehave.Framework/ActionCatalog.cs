using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                actionExist = (FindMathingAction(message) != null);
            return actionExist;
        }

        public bool CatalogedActionIsTokenized(string message)
        {
            string catalogedAction = FindMathingAction(message);
            return (catalogedAction != null && catalogedAction.IndexOf(TokenPrefix) > -1);
        }

        private object GetActionFromCatalog(string message)
        {
            string keyToUse = message;
            bool actionExist = _actions.ContainsKey(keyToUse);
            if (actionExist == false)
            {
                keyToUse = FindMathingAction(message);
                actionExist = (keyToUse != null);
            }
            if (actionExist)
                return _actions[keyToUse];
            return null;
        }

        private string FindMathingAction(string message)
        {
            string[] messageWords = SplitWordsToArray(message);
            string mathingMessage = null;
            foreach (string key in _actions.Keys)
            {
                string[] words = SplitWordsToArray(key);
                if (messageWords.Length == words.Length)
                {
                    bool allEqual = true;
                    for (int i = 0; i < messageWords.Length; i++)
                    {
                        if (words[i].IndexOf(TokenPrefix) == -1)
                        {
                            allEqual &= messageWords[i].Equals(words[i], StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                    if (allEqual)
                    {
                        mathingMessage = key;
                    }
                }
            }
            return mathingMessage;
        }

        private string[] SplitWordsToArray(string message)
        {
            return message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public object GetAction(string message)
        {
            return GetActionFromCatalog(message);
        }

        public object[] GetParametersForMessage(string message)
        {
            string[] messageAction = SplitWordsToArray(message);
            string[] catalogedAction = SplitWordsToArray(FindMathingAction(message));
            object action = GetAction(message);

            Type[] args = action.GetType().GetGenericArguments();
            object[] values = new object[args.Length];
            int argNumber = 0;
            for (int i = 0; i < catalogedAction.Length; i++)
            {
                if (catalogedAction[i].StartsWith(TokenPrefix.ToString()))
                {
                    values[argNumber] = Convert.ChangeType(messageAction[i], args[argNumber]); //convert string to an instance of args[argNumber]
                    argNumber++;
                }
            }
            return values;
        }

        public string BuildMessage(string message, object[] parameters)
        {
            string resultString = message;
            for (int i = 0; i < parameters.Length; i++)
            {
                int keyWordStartPos = resultString.IndexOf(TokenPrefix);
                if (keyWordStartPos > -1)
                {
                    int keyWordEnd = resultString.IndexOf(' ', keyWordStartPos);
                    if (keyWordEnd == -1)
                        keyWordEnd = resultString.Length;
                    string keyWord = resultString.Substring(keyWordStartPos, keyWordEnd - keyWordStartPos);
                    resultString = resultString.Replace(keyWord, parameters[i].ToString());
                }
            }
            return resultString;
        }
    }
}
