using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class IgnoreSpaceAndUnderScoreMatcher : IFileMatcher
    {
        private readonly string _className;
		
        public IgnoreSpaceAndUnderScoreMatcher(Type typeToMatch)
        {
            _className = typeToMatch.Name.Replace("_","").Replace(" ","");
        }
		
        bool IFileMatcher.IsMatch(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string match = Regex.Match(fileNameWithoutExtension, @"^[\w+_|\s]*$").Value;
            string matchWithoutSpaceAndUnderScore = match.Replace("_","").Replace(" ","");
            return _className == matchWithoutSpaceAndUnderScore;
        }

    }
}