using System;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public class ReflectedMethodStepResolver : IStepResolver
    {
        private readonly object _methodProvider;

        public ReflectedMethodStepResolver(object methodProvider)
        {
            _methodProvider = methodProvider;
        }

        public Action ResolveStep(ScenarioFragment currentScenarioStage, ActionStepText actionStep)
        {
            var methodName = currentScenarioStage + "_" + actionStep.Step.Replace(' ', '_');
            Type storyType = _methodProvider.GetType();
            MethodInfo method = storyType.GetMethod(methodName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                                                    null, Type.EmptyTypes, null);
            if (method != null)
            {
                return () => method.Invoke(_methodProvider, new object[0]);
            }

            return null;
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
