// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

namespace NBehave.Extensions
{
    public static class ActionStepConverterExtensions
    {
        private const string TokenRegexPattern = @"(?<name>\$[a-zA-Z]\w*)|(?<bracketName>(\[|\<)[a-zA-Z]\w*(\]|\>))";
        private static readonly Regex TokenPattern = new Regex(TokenRegexPattern);
        private static readonly Regex ValidRegexGroupName = new Regex(@"[a-zA-Z]\w*");

        public static Regex AsRegex(this string actionStep)
        {
            var words = actionStep.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var regex = "^";
            foreach (var w in words)
            {
                var word = w.Replace("(", "\\(").Replace(")", "\\)");
                if (!WordIsToken(word))
                {
                    regex += string.Format(@"{0}\s+", word);
                    continue;
                }
                var handleBracketName = TokenPattern.Match(word).Groups["bracketName"].Success ? 1 : 0;
                var groupName = GetValidRegexGroupName(word);
                var stuffAtStart = word.Substring(0, word.IndexOf(groupName) - 1);
                var stuffAtEnd = word.Substring(word.IndexOf(groupName) + groupName.Length + handleBracketName);

                var lengthRestriction = "+";
                if (stuffAtEnd.StartsWith("{") && stuffAtEnd.Contains("}"))
                {
                    lengthRestriction = stuffAtEnd.Substring(0, stuffAtEnd.IndexOf("}") + 1);
                    stuffAtEnd = stuffAtEnd.Remove(0, lengthRestriction.Length);
                }

                regex += string.Format(@"{1}(?<{0}>.{3}){2}\s+", groupName, stuffAtStart, stuffAtEnd, lengthRestriction);
            }

            if (regex.EndsWith(@"\s+"))
            {
                regex = regex.Substring(0, regex.Length - 1) + "*";
            }

            regex += "$";
            return new Regex(regex, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        private static bool WordIsToken(string word)
        {
            return TokenPattern.IsMatch(word);
        }

        private static string GetValidRegexGroupName(string word)
        {
            return ValidRegexGroupName.Match(word).Value;
        }

        private static readonly Regex IsCharsAndNumbersOnly = new Regex(@"^(\w|\d|\s)+$");
        private static readonly Regex IsTokenString = new Regex(TokenRegexPattern);
        private static readonly Regex IsRegexWithNamedGroup = new Regex(@"\(\?\<\w+\>");
        public static bool IsRegex(this string regexOrTokenString)
        {
            if (string.IsNullOrEmpty(regexOrTokenString))
                return false;

            if (regexOrTokenString.EndsWith("$") 
                || regexOrTokenString.StartsWith("^")
                || IsRegexWithNamedGroup.IsMatch(regexOrTokenString))
            {
                return true;
            }

            if (IsTokenString.IsMatch(regexOrTokenString)
                || IsCharsAndNumbersOnly.IsMatch(regexOrTokenString))
            {
                return false;
            }

            return true;
        }
    }
}
