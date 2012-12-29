using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;

namespace NBehave.Fluent.Framework
{
    public class DictionaryStepResolver : IStepResolver
    {
        private readonly Dictionary<string, Action> inlineImplementations = new Dictionary<string, Action>();

        public void RegisterImplementation(ScenarioFragment currentScenarioStage, string step, Action implementation)
        {
            if (implementation != null)
            {
                var stepKey = currentScenarioStage + " " + step;
                RegisterImpl(stepKey, implementation);
                RegisterImpl("And " + step, implementation);
            }
        }

        private void RegisterImpl(string stepKey, Action implementation)
        {
            if (inlineImplementations.ContainsKey(stepKey))
                inlineImplementations.Remove(stepKey);
            inlineImplementations.Add(stepKey, implementation);
        }

        public Action ResolveStep(StringStep stringStep)
        {
            Action step;
            return inlineImplementations.TryGetValue(stringStep.Step, out step) ? step : null;
        }

        public Action ResolveOnCloseScenario()
        {
            return null;
        }

        public Action ResolveOnBeforeScenario()
        {
            return null;
        }

        public Action ResolveOnAfterScenario()
        {
            return null;
        }
    }
}
