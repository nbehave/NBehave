using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    public class ScenarioDrivenSpecStepRunner : IStringStepRunner
    {
        private readonly DictionaryStepResolver _inlineImplementations;
        private readonly List<IStepResolver> _resolvers;

        public ScenarioFragment CurrentScenarioStage { get; set; }

        public ScenarioDrivenSpecStepRunner(object stepHelper)
        {
            _inlineImplementations = new DictionaryStepResolver();
            _resolvers = new List<IStepResolver>
                             {
                                 _inlineImplementations
                             };

            if (stepHelper == null)
                return;

            if (stepHelper.GetType()
                .GetCustomAttributes(typeof(ActionStepsAttribute), true)
                .Length > 0)
            {
                _resolvers.Add(new ActionStepStepResolver(stepHelper));
            }
            else
            {
                _resolvers.Add(new ReflectedMethodStepResolver(stepHelper));
            }
        }

        public void RegisterImplementation(string step, Action implementation)
        {
            _inlineImplementations.RegisterImplementation(CurrentScenarioStage, step, implementation);
        }

        public ActionStepResult Run(ActionStepText actionStep)
        {
            var stepText = actionStep.Step;

            var stepImplementation = _resolvers.Select(resolver => resolver.ResolveStep(CurrentScenarioStage, actionStep))
                                                  .Where(action => action != null)
                                                  .FirstOrDefault();
            if (stepImplementation == null)
            {
                return new ActionStepResult(stepText, new Pending("No implementation located"));
            }

            try
            {
                stepImplementation();
                return new ActionStepResult(stepText, new Passed());
            }
            catch (Exception ex)
            {
                var realException = FindUsefulException(ex);
                return new ActionStepResult(stepText, new Failed(realException));
            }
        }

        public ActionStepResult Run(ActionStepText actionStep, Row row)
        {
            throw new NotSupportedException("NBehave.Spec does not support example-driven scenarios");
        }

        public void OnCloseScenario()
        {
            var closeAction = _resolvers.Select(resolver => resolver.ResolveOnCloseScenario())
                                                  .Where(action => action != null)
                                                  .FirstOrDefault();
            if (closeAction != null)
                closeAction();
        }

        public void BeforeScenario()
        {
            var beforeAction = _resolvers.Select(resolver => resolver.ResolveOnBeforeScenario())
                                                  .Where(action => action != null)
                                                  .FirstOrDefault();
            if (beforeAction != null)
                beforeAction();
        }

        public void AfterScenario()
        {
            var afterAction = _resolvers.Select(resolver => resolver.ResolveOnAfterScenario())
                                                  .Where(action => action != null)
                                                  .FirstOrDefault();
            if (afterAction != null)
                afterAction();
        }

        private Exception FindUsefulException(Exception e)
        {
            var realException = e;
            while (realException != null && realException.GetType() == typeof(TargetInvocationException))
            {
                realException = realException.InnerException;
            }
            if (realException == null)
                return e;
            return realException;
        }
    }
}
