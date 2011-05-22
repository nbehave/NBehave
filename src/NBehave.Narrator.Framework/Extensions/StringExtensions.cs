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
        private static readonly Regex _whiteSpace = new Regex(@"\s", RegexOptions.Compiled);

        public static string ReplaceFirst(this string str, string toReplace, string replaceWith)
        {
            var start = str.IndexOf(toReplace);
            if (start < 0)
            {
                return str;
            }

            var newString = str.Remove(start, toReplace.Length);
            return newString.Insert(start, replaceWith);
        }

        public static string GetFirstWord(this string str)
        {
            var regex = new Regex(@"(\w+|\d+)+");
            return regex.Match(str).Value;
        }

        public static string RemoveFirstWord(this string tokenString)
        {
            var tokenStringToMatch = tokenString.TrimStart();
            var firstWhiteSpace = _whiteSpace.Match(tokenStringToMatch);
            if (firstWhiteSpace.Success)
            {
                var posOfFirstSpace = firstWhiteSpace.Index;
                return tokenStringToMatch.Substring(posOfFirstSpace + 1).TrimStart(new[] { ' ', '\n', '\r', '\t' });
            }

            return tokenString;
        }
    }
}
