// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringStepRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringStepRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Globalization;
    using System.Reflection;

    public class StringStepRunner : IStringStepRunner
    {
        private ActionMethodInfo _lastAction;

        private bool _isFirstStepInScenario = true;

        public StringStepRunner(ActionCatalog actionCatalog)
        {
            ActionCatalog = actionCatalog;
            ParameterConverter = new ParameterConverter(ActionCatalog);
        }

        private ActionCatalog ActionCatalog { get; set; }

        private ParameterConverter ParameterConverter { get; set; }

        public StepResult Run(ActionStepText actionStep)
        {
            return (this as IStringStepRunner).Run(actionStep, null);
        }

        public StepResult Run(ActionStepText actionStep, Row row)
        {
            var actionStepToUse = new ActionStepText(actionStep.Step.RemoveFirstWord(), actionStep.Source);
            var result = new StepResult(actionStep.Step, new Passed());
            try
            {
                if (!ActionCatalog.ActionExists(actionStepToUse))
                {
                    var pendReason = string.Format("No matching Action found for \"{0}\"", actionStep);
                    result = new StepResult(actionStep.Step, new Pending(pendReason));
                }
                else
                {
                    if (row == null)
                    {
                        RunStep(actionStepToUse);
                    }
                    else
                    {
                        RunStep(actionStepToUse, row);
                    }
                }
            }
            catch (Exception e)
            {
                var realException = FindUsefulException(e);
                result = new StepResult(actionStep.Step, new Failed(realException));
            }

            return result;
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

        private void RunStep(ActionStepText actionStepToUse)
        {
            Func<object[]> getParameters = () => ParameterConverter.GetParametersForActionStepText(actionStepToUse);
            RunStep(actionStepToUse, getParameters);
        }

        private void RunStep(ActionStepText actionStep, Row row)
        {
            Func<object[]> getParameters = () => ParameterConverter.GetParametersForActionStepText(actionStep, row);
            RunStep(actionStep, getParameters);
        }

        private void RunStep(ActionStepText actionStep, Func<object[]> getParametersForActionStepText)
        {
            if (ActionCatalog.ActionExists(actionStep) == false)
            {
                throw new ArgumentException(string.Format("cannot find step string '{0}'", actionStep));
            }

            var info = ActionCatalog.GetAction(actionStep);

            BeforeEachScenario(info);
            BeforeEachStep(info);
            RunStep(info, getParametersForActionStepText);
            AfterEachStep(info);

            _lastAction = info;
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