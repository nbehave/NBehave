using System;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public interface IStepResolver
    {
        Action ResolveStep(ScenarioFragment currentScenarioStage, ActionStepText actionStep);
        Action ResolveOnBeforeScenario();
        Action ResolveOnAfterScenario();
    }
}
