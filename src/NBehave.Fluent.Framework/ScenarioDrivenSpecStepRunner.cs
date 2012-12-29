using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Fluent.Framework
{
    public class ScenarioDrivenSpecStepRunner : IStringStepRunner
    {
        private readonly DictionaryStepResolver _inlineImplementations;
        private readonly List<IStepResolver> resolvers;

        public ScenarioDrivenSpecStepRunner(object stepHelper)
        {
            _inlineImplementations = new DictionaryStepResolver();
            resolvers = new List<IStepResolver>
                             {
                                 _inlineImplementations
                             };

            if (stepHelper == null)
                return;

            if (stepHelper.GetType()
                .GetCustomAttributes(typeof(ActionStepsAttribute), true)
                .Length > 0)
            {
                resolvers.Add(new ActionStepStepResolver(stepHelper));
            }
            else
            {
                resolvers.Add(new ReflectedMethodStepResolver(stepHelper));
            }
        }

        public void RegisterImplementation(ScenarioFragment scenarioStage, string step, Action implementation)
        {
            _inlineImplementations.RegisterImplementation(scenarioStage, step, implementation);
        }

        public void Run(StringStep step)
        {
            var stepImplementation = resolvers
                .Select(resolver => resolver.ResolveStep(step))
                .FirstOrDefault(action => action != null);
            if (stepImplementation == null)
            {
                step.PendNotImplemented("No implementation found");
                return;
            }

            try
            {
                stepImplementation();
            }
            catch (Exception ex)
            {
                var realException = FindUsefulException(ex);
                step.Fail(realException);
            }
        }

        public void Run(StringStep step, Example example)
        {
            throw new NotSupportedException("NBehave.Spec does not support example-driven scenarios");
        }

        public void OnCloseScenario()
        {
            var closeAction = resolvers
                .Select(resolver => resolver.ResolveOnCloseScenario())
                .FirstOrDefault(action => action != null);
            if (closeAction != null)
                closeAction();
        }

        public void BeforeScenario()
        {
            var beforeAction = resolvers
                .Select(resolver => resolver.ResolveOnBeforeScenario())
                .FirstOrDefault(action => action != null);
            if (beforeAction != null)
                beforeAction();
        }

        public void AfterScenario()
        {
            var afterAction = resolvers
                .Select(resolver => resolver.ResolveOnAfterScenario())
                .FirstOrDefault(action => action != null);
            if (afterAction != null)
                afterAction();
        }

        private Exception FindUsefulException(Exception e)
        {
            var realException = e;
            while (realException is TargetInvocationException)
            {
                realException = realException.InnerException;
            }
            if (realException == null)
                return e;
            return realException;
        }
    }
}
