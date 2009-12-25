using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    internal static class ActionStepConverterExtensions
    {
        public const string TokenRegexPattern = @"(\$[a-zA-Z]\w+)|(\[[a-zA-Z]\w+\])";
        private static readonly Regex _tokenPattern = new Regex(TokenRegexPattern);
        private static readonly Regex _validRegexGroupName = new Regex(@"[a-zA-Z]\w+");

        public static Regex AsRegex(this string actionStep)
        {
            string[] words = actionStep.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string regex = "^";
            foreach (var word in words)
            {
                if (WordIsToken(word))
                {
                    var groupName = GetValidRegexGroupName(word);
                    var stuffAtStart = word.Substring(0, word.IndexOf(groupName) - 1);
                    var stuffAtEnd = word.Substring(word.IndexOf(groupName) + groupName.Length);
                    regex += string.Format(@"{1}(?<{0}>.+){2}\s+", groupName, stuffAtStart, stuffAtEnd);
                }
                else
                    regex += string.Format(@"{0}\s+", word);
            }
            if (regex.EndsWith(@"\s+"))
                regex = regex.Substring(0, regex.Length - 1) + "*";
            regex += "$";
            return new Regex(regex, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        private static bool WordIsToken(string word)
        {
            return _tokenPattern.IsMatch(word);
        }

        private static string GetValidRegexGroupName(string word)
        {
            return _validRegexGroupName.Match(word).Value;
        }
    }
}
