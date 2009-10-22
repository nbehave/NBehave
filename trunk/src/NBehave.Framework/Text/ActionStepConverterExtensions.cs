using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    internal static class ActionStepConverterExtensions
    {

        public static Regex AsRegex(this string actionStep)
        {
            string[] words = actionStep.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string regex = "^";
            foreach (var word in words)
            {
                if (WordIsToken(word))
                {
                    var groupName = GetValidRegexGroupName(word);
                    var stuffAtEnd = RemoveTokenPrefix(word).Replace(groupName, string.Empty);
                    regex += string.Format(@"(?<{0}>.+){1}\s+", groupName, stuffAtEnd);
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
            return word.StartsWith(ActionCatalog.TokenPrefix.ToString());
        }

        private static string GetValidRegexGroupName(string word)
        {
            var regex = new Regex(@"\w+");
            return regex.Match(word).Value;
        }

        private static string RemoveTokenPrefix(string word)
        {
            return word.Substring(ActionCatalog.TokenPrefix.ToString().Length);
        }
    }
}
