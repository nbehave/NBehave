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
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class StringStepRunner : IStringStepRunner
    {
        private ActionMethodInfo _lastAction;

        private bool _isFirstStepInScenario = true;
        private readonly ITinyMessengerHub _hub;

        public StringStepRunner(ActionCatalog actionCatalog, ITinyMessengerHub hub)
        {
            ActionCatalog = actionCatalog;
            _hub = hub;
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

        public void OnCloseScenario()
        {
            if (_lastAction != null)
            {
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
            }
        }

        public void BeforeScenario()
        {
            _isFirstStepInScenario = true;
        }

        public void AfterScenario()
        {
            if (_lastAction != null)
            {
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
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

            try
            {
                PublishStepStartedEvent(actionStep);
                BeforeEachScenario(info);
                BeforeEachStep(info);
                if (actionStep is StringTableStep && ShouldForEachOverStep(info))
                    ForEachOverStep(actionStep as StringTableStep, info);
                else
                    RunStep(info, () => ParameterConverter.GetParametersForStep(actionStep));
            }
            finally
            {
                _lastAction = info;
                AfterEachStep(info);
                PublishStepFinishedEvent(actionStep);
            }
        }

        private void ForEachOverStep(StringTableStep actionStep, ActionMethodInfo info)
        {
            foreach (var example in actionStep.TableSteps)
                RunStep(info, () => ParameterConverter.GetParametersForStep(actionStep, example));
        }

        private static bool ShouldForEachOverStep(ActionMethodInfo info)
        {
            return info.MethodParametersType != MethodParametersType.TypedListStep;
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

        private void PublishStepStartedEvent(StringStep actionStep)
        {
            _hub.Publish(new StepStartedEvent(this, actionStep));
        }

        private void PublishStepFinishedEvent(StringStep actionStep)
        {
            _hub.Publish(new StepFinishedEvent(this, actionStep.Step));
        }

        private void BeforeEachStep(ActionMethodInfo info)
        {
            info.ExecuteNotificationMethod(typeof(BeforeStepAttribute));
        }

        private void AfterEachStep(ActionMethodInfo info)
        {
            info.ExecuteNotificationMethod(typeof(AfterStepAttribute));
        }

        private void BeforeEachScenario(ActionMethodInfo info)
        {
            if (_isFirstStepInScenario)
            {
                _isFirstStepInScenario = false;
                info.ExecuteNotificationMethod(typeof(BeforeScenarioAttribute));
            }
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