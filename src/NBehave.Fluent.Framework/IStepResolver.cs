using System;

namespace NBehave.Fluent.Framework
{
    public interface IStepResolver
    {
        Action ResolveStep(StringStep stringStep);
    }
}
