using System;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Fluent.Framework
{
    public class ReflectedMethodStepResolver : IStepResolver
    {
        private readonly object _methodProvider;

        public ReflectedMethodStepResolver(object methodProvider)
        {
            _methodProvider = methodProvider;
        }

        public Action ResolveStep(ActionStepText actionStep)
        {
            var methodName = actionStep.Step.Replace(' ', '_');
            var storyType = _methodProvider.GetType();
            var method = storyType.GetMethod(methodName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
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
