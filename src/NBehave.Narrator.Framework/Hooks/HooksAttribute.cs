using System;
using JetBrains.Annotations;

namespace NBehave.Narrator.Framework.Hooks
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class HooksAttribute : Attribute { }
}