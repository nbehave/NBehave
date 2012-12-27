// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringStepRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringStepRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;

namespace NBehave.Internal
{
    public class StringStepRunner : IStringStepRunner
    {
        public StringStepRunner(ActionCatalog actionCatalog)
        {
            ActionCatalog = actionCatalog;
            ParameterConverter = new ParameterConverter(ActionCatalog);
        }

        private ActionCatalog ActionCatalog { get; set; }

        private ParameterConverter ParameterConverter { get; set; }

        public void Run(StringStep step)
        {
            try
            {
                if (!ActionCatalog.ActionExists(step))
                {
                    var pendReason = string.Format("No matching Action found for \"{0}\"", step);
                    step.PendNotImplemented(pendReason);
                }
                else
                {
                    RunStep(step);
                }
            }
            catch (Exception e)
            {
                var realException = FindUsefulException(e);
                step.Fail(realException);
            }
        }

        private Type GetActionType(object action)
        {
            var actionType = action.GetType().IsGenericType
                ? action.GetType().GetGenericTypeDefinition()
                : action.GetType();
            return actionType;
        }

        private void RunStep(StringStep actionStep)
        {
            var info = ActionCatalog.GetAction(actionStep);

            if (actionStep is StringTableStep && !ShouldForEachOverStep(info))
                ForEachOverStep(actionStep as StringTableStep, info);
            else
                RunStep(info, () => ParameterConverter.GetParametersForStep(actionStep));
        }

        private void ForEachOverStep(StringTableStep actionStep, ActionMethodInfo info)
        {
            foreach (var example in actionStep.TableSteps)
                RunStep(info, () => ParameterConverter.GetParametersForStep(actionStep, example));
        }

        private bool ShouldForEachOverStep(ActionMethodInfo info)
        {
            return info.MethodParametersType == MethodParametersType.TypedListStep;
        }

        private void RunStep(ActionMethodInfo info, Func<object[]> getParametersForActionStepText)
        {
            var actionType = GetActionType(info.Action);
            var methodInfo = actionType.GetMethod("DynamicInvoke");
            var actionParamValues = getParametersForActionStepText();
            methodInfo.Invoke(
                info.Action,
                BindingFlags.InvokeMethod,
                null,
                new object[] { actionParamValues },
                CultureInfo.CurrentCulture);
        }

        private Exception FindUsefulException(Exception e)
        {
            var realException = e;
            while (realException != null && realException.GetType() == typeof(TargetInvocationException))
            {
                realException = realException.InnerException;
            }

            if (realException == null)
            {
                return e;
            }

            return realException;
        }
    }
}