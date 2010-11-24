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
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var match = Regex.Match(fileNameWithoutExtension, @"^[\w+_|\s]*$").Value;
            var matchWithoutSpaceAndUnderScore = match.Replace("_","").Replace(" ","");
            return _className == matchWithoutSpaceAndUnderScore;
        }

    }
}