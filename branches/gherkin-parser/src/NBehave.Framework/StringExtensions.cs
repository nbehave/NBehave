using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{

    public static class StringExtensions
    {
        public static string ReplaceFirst(this string str, string toReplace, string replaceWith)
        {
            var start = str.IndexOf(toReplace);
            if (start < 0)
                return str;

            var newString = str.Remove(start, toReplace.Length);
            return newString.Insert(start, replaceWith);
        }

        public static string GetFirstWord(this string str)
        {
            var regex = new Regex(@"(\w+|\d+)+");
            return regex.Match(str).Value;
        }

        private static readonly Regex _whiteSpace = new Regex(@"\s", RegexOptions.Compiled);
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
