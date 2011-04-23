using System;
using System.Globalization;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class StringStepRunner : IStringStepRunner
    {
        private ActionCatalog ActionCatalog { get; set; }
        private ParameterConverter ParameterConverter { get; set; }
        private ActionMethodInfo _lastAction;
        private bool _isFirstStepInScenario = true;

        public StringStepRunner(ActionCatalog actionCatalog)
        {
            ActionCatalog = actionCatalog;
            ParameterConverter = new ParameterConverter(ActionCatalog);
        }

        ActionStepResult IStringStepRunner.Run(ActionStepText actionStep)
        {
            return (this as IStringStepRunner).Run(actionStep, null);
        }

        ActionStepResult IStringStepRunner.Run(ActionStepText actionStep, Row row)
        {
            var actionStepToUse = new ActionStepText(actionStep.Step.RemoveFirstWord(), actionStep.Source);
            var result = new ActionStepResult(actionStep.Step, new Passed());
            try
            {
                if (ActionCatalog.ActionExists(actionStepToUse) == false)
                {
                    string pendReason = string.Format("No matching Action found for \"{0}\"", actionStep);
                    result = new ActionStepResult(actionStep.Step, new Pending(pendReason));
                }
                else
                {
                    if (row == null)
                        RunStep(actionStepToUse);
                    else
                        RunStep(actionStepToUse, row);
                }
            }
            catch (Exception e)
            {
                Exception realException = FindUsefulException(e);
                result = new ActionStepResult(actionStep.Step, new Failed(realException));
            }
            return result;
        }


        void IStringStepRunner.BeforeScenario()
        {
            _isFirstStepInScenario = true;
        }

        void IStringStepRunner.AfterScenario()
        {
            if (_lastAction != null)
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
        }

        private Type GetActionType(object action)
        {
            Type actionType = action.GetType().IsGenericType
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
                throw new ArgumentException(string.Format("cannot find step string '{0}'", actionStep));

            var info = ActionCatalog.GetAction(actionStep);

            BeforeEachScenario(info);
            BeforeEachStep(info);
            RunStep(info, getParametersForActionStepText);
            AfterEachStep(info);

            _lastAction = info;
        }

        private void RunStep(ActionMethodInfo info, Func<object[]> getParametersForActionStepText)
        {
            Type actionType = GetActionType(info.Action);
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = getParametersForActionStepText();
            methodInfo.Invoke(info.Action, BindingFlags.InvokeMethod, null,
                              new object[] { actionParamValues }, CultureInfo.CurrentCulture);
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
            Exception realException = e;
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