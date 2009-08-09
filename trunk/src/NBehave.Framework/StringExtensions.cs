using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{

    public static class StringExtensions
    {
        public static string ReplaceFirst(this string str, string toReplace, string replaceWith)
        {
            int start = str.IndexOf(toReplace);
            if (start < 0)
                return str;

            string newString = str.Remove(start, toReplace.Length);
            return newString.Insert(start, replaceWith);
        }

        public static string GetFirstWord(this string str)
        {
            var regex = new Regex(@"(\w+|\d+)+");
            return regex.Match(str).Value;
        }

        private static Regex _whiteSpace = new Regex(@"\s", RegexOptions.Compiled);
        public static string RemoveFirstWord(this string tokenString)
        {
            Match firstWhiteSpace = _whiteSpace.Match(tokenString);
            if (firstWhiteSpace.Success)
            {
                int posOfFirstSpace = firstWhiteSpace.Index;
                return tokenString.Substring(posOfFirstSpace + 1).TrimStart(new char[] { ' ', '\n', '\r' });
            }
            return tokenString;
        }
    }
}
