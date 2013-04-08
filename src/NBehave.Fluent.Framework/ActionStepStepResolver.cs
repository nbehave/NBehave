using System;
using NBehave.Hooks;
using NBehave.Internal;

namespace NBehave.Fluent.Framework
{
    public class ActionStepStepResolver : IStepResolver
    {
        private readonly object _stepHelper;
        private readonly ActionCatalog _actionCatalog;
        private readonly ActionStepParser _stepParser;
        private readonly ParameterConverter _parameterConverter;

        public ActionStepStepResolver(object stepHelper)
        {
            _stepHelper = stepHelper;
            _actionCatalog = new ActionCatalog();
            _stepParser = new ActionStepParser(new StoryRunnerFilter(), _actionCatalog);
            _stepParser.FindActionStepMethods(stepHelper.GetType(), _stepHelper);
            _parameterConverter = new ParameterConverter(_actionCatalog);
        }

        public Action ResolveStep(StringStep stringStep)
        {
            var actionMethodInfo = _actionCatalog.GetAction(stringStep);
            if (actionMethodInfo == null)
                return null;

            var parameters = _parameterConverter.GetParametersForStep(stringStep);
            return () =>
                       {
                           actionMethodInfo.ExecuteNotificationMethod(typeof(BeforeStepAttribute));
                           actionMethodInfo.MethodInfo.Invoke(_stepHelper, parameters);
                           actionMethodInfo.ExecuteNotificationMethod(typeof(AfterStepAttribute));
                       };
        }
    }
}
