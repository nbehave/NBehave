using System;
using NBehave.Narrator.Framework;

namespace NBehave.Fluent.Framework
{
    public interface IStepResolver
    {
        Action ResolveStep(StringStep stringStep);
        Action ResolveOnCloseScenario();
        Action ResolveOnBeforeScenario();
        Action ResolveOnAfterScenario();
    }
}
