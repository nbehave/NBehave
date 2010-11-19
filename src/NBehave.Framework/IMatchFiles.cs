// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMatchFiles.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the IMatchFiles type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public interface IMatchFiles
    {
        IFileMatcher FileMatcher { get; }
    }
}
