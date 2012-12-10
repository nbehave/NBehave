// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchAllFiles.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the MatchAllFiles type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Contracts;

namespace NBehave
{
    public class MatchAllFiles : IFileMatcher
    {
        bool IFileMatcher.IsMatch(string fileName)
        {
            return true;
        }
    }
}