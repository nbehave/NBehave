// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
