using System;
using JetBrains.Annotations;

namespace NBehave.Hooks
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class HooksAttribute : Attribute { }
}