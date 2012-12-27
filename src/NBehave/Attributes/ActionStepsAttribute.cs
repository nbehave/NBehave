using JetBrains.Annotations;

using System;

namespace NBehave
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class ActionStepsAttribute : Attribute
    {
    }
}
