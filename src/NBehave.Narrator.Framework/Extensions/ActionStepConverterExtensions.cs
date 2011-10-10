// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Text.RegularExpressions;

    public static class ActionStepConverterExtensions
    {
        public const string TokenRegexPattern = @"(?<name>\$[a-zA-Z]\w*)|(?<bracketName>\[[a-zA-Z]\w*\])";
        private static readonly Regex _tokenPattern = new Regex(TokenRegexPattern);
        private static readonly Regex _validRegexGroupName = new Regex(@"[a-zA-Z]\w*");

        public static Regex AsRegex(this string actionStep)
        {
            var words = actionStep.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var regex = "^";
            foreach (var word in words)
            {
                if (!WordIsToken(word))
                {
                    regex += string.Format(@"{0}\s+", word);
                    continue;
                }
                var handleBracketName = _tokenPattern.Match(word).Groups["bracketName"].Success ? 1 : 0;
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
            return _tokenPattern.IsMatch(word);
        }

        private static string GetValidRegexGroupName(string word)
        {
            return _validRegexGroupName.Match(word).Value;
        }
    }
}
