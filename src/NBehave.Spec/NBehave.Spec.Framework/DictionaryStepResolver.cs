using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public class DictionaryStepResolver : IStepResolver
    {
        private readonly Dictionary<string, Action> _inlineImplementations = new Dictionary<string, Action>();

        public void RegisterImplementation(ScenarioFragment currentScenarioStage, string step, Action implementation)
        {
            if (implementation != null)
            {
                string stepKey = currentScenarioStage + " " + step;
                if (_inlineImplementations.ContainsKey(stepKey))
                    _inlineImplementations.Remove(stepKey);
                _inlineImplementations.Add(stepKey, implementation);
            }
        }
        
        public Action ResolveStep(ScenarioFragment currentScenarioStage, ActionStepText actionStep)
        {
            string stepText = currentScenarioStage + " " + actionStep.Step;
            if (_inlineImplementations.ContainsKey(stepText))
            {
                return _inlineImplementations[stepText];
            }
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
