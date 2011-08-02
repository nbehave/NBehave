// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static readonly char[] WhiteSpaceChars = new[] { ' ', '\n', '\r', '\t' };

        private static readonly Regex WhiteSpace = new Regex(@"\s", RegexOptions.Compiled);

        public static string TrimWhiteSpaceChars(this string str)
        {
            return str.Trim(WhiteSpaceChars);
        }

        public static string GetFirstWord(this string str)
        {
            var regex = new Regex(@"(\w+|\d+)+");
            return regex.Match(str).Value;
        }

        public static string RemoveFirstWord(this string tokenString)
        {
            var tokenStringToMatch = tokenString.TrimStart();
            var firstWhiteSpace = WhiteSpace.Match(tokenStringToMatch);
            if (firstWhiteSpace.Success)
            {
                var posOfFirstSpace = firstWhiteSpace.Index;
                return tokenStringToMatch.Substring(posOfFirstSpace + 1).TrimStart(WhiteSpaceChars);
            }

            return tokenString;
        }

        public static ActionStepText AsActionStepText(this string text, string source)
        {
            return new StringStep(text, source);
        }
    }
}
