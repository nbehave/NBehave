// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStringStepRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the IStringStepRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Domain;

namespace NBehave.Internal
{
    public interface IStringStepRunner
    {
        void Run(StringStep step);
    }
}