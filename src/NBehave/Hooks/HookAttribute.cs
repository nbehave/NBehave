using System;
using JetBrains.Annotations;

namespace NBehave.Narrator.Framework.Hooks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [MeansImplicitUse]
    public class HookAttribute : Attribute
    {
        public string[] RunIfHasTags { get; set; }
        public string[] DontRunIfHasTags { get; set; }

        protected HookAttribute()
            : this(new string[0], new string[0])
        { }

        private HookAttribute(string[] runIfHasTags, string[] dontRunIfHasTags)
        {
            RunIfHasTags = runIfHasTags;
            DontRunIfHasTags = dontRunIfHasTags;
        }
    }
}