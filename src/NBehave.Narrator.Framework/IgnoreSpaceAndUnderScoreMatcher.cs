// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IgnoreSpaceAndUnderScoreMatcher.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the IgnoreSpaceAndUnderScoreMatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    public class IgnoreSpaceAndUnderScoreMatcher : IFileMatcher
    {
        private readonly string _className;
        
        public IgnoreSpaceAndUnderScoreMatcher(Type typeToMatch)
        {
            _className = typeToMatch.Name.Replace("_", string.Empty).Replace(" ", string.Empty);
        }
        
        bool IFileMatcher.IsMatch(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var match = Regex.Match(fileNameWithoutExtension, @"^[\w+_|\s]*$").Value;
            var matchWithoutSpaceAndUnderScore = match.Replace("_", string.Empty).Replace(" ", string.Empty);
            return _className == matchWithoutSpaceAndUnderScore;
        }
    }
}