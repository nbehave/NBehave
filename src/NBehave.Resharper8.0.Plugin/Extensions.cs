using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Extensions;

namespace NBehave.ReSharper.Plugin
{
    public static class Extensions
    {
        public static string AsString(this IEnumerable<Example> examples)
        {
            var s = "";
            examples.Each(e => s += e.ToString() + Environment.NewLine);
            return s;
        }
    }
}
