// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeforeScenarioAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the BeforeScenarioAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NBehave.Narrator.Framework
{
    [Obsolete("Use NBehave.Narrator.Framework.Hooks.BeforeScenarioAttribute")]
    public class BeforeScenarioAttribute : NotificationMethodAttribute
    {
    }
}