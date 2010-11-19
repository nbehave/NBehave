// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileMatcher.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the IFileMatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public interface IFileMatcher
    {
        bool IsMatch(string fileName);
    }
}