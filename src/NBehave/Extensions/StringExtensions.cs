// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using NBehave.Domain;

namespace NBehave.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] WhiteSpaceChars = new[] { ' ', '\n', '\r', '\t' };
        private static readonly Regex WhiteSpace = new Regex(@"\s");

        public static string TrimWhiteSpaceChars(this string str)
        {
            return str.Trim(WhiteSpaceChars);
        }

        private static readonly Regex FirstWordRegex = new Regex(@"(\w+|\d+)+", RegexOptions.Compiled);
        public static string GetFirstWord(this string str)
        {
            return FirstWordRegex.Match(str).Value;
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

        public static StringStep AsStringStep(this string text, string source)
        {
            return new StringStep(text, source);
        }
    }
}
