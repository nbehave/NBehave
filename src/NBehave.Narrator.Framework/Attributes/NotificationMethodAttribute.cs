// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationMethodAttribute.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the NotificationMethodAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Obsolete("Use attributes of type NBehave.Narrator.Framework.Hooks.HookAttribute")]
    public class NotificationMethodAttribute : Attribute
    {
    }
}