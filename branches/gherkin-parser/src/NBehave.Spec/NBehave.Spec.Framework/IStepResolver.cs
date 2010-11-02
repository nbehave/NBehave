using System;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public interface IStepResolver
    {
        Action ResolveStep(ScenarioFragment currentScenarioStage, ActionStepText actionStep);
        Action ResolveOnCloseScenario();
        Action ResolveOnBeforeScenario();
        Action ResolveOnAfterScenario();
    }
}
