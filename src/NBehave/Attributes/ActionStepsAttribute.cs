// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepsAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepsAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using JetBrains.Annotations;

    using System;

namespace NBehave.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class ActionStepsAttribute : Attribute
    {
    }
}
