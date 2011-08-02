using System;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public interface IStepResolver
    {
        Action ResolveStep(ActionStepText actionStep);
        Action ResolveOnCloseScenario();
        Action ResolveOnBeforeScenario();
        Action ResolveOnAfterScenario();
    }
}
