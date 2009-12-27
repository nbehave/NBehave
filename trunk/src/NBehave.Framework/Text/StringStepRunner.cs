using System;
using System.Globalization;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class StringStepRunner
    {
        private readonly ActionCatalog _actionCatalog;
        private ActionMethodInfo _lastAction;
        private bool _isFirstStepInScenario = true;

        public StringStepRunner(ActionCatalog actionCatalog)
        {
            _actionCatalog = actionCatalog;
        }

        public void InvokeStringStep(ActionStepText actionStep)
        {
            Func<object[]> getParameters = () => _actionCatalog.GetParametersForActionStepText(actionStep);
            InvokeStringStep(actionStep, getParameters);
        }

        private void InvokeStringStep(ActionStepText actionStep, Row row)
        {
            Func<object[]> getParameters = () => _actionCatalog.GetParametersForActionStepText(actionStep, row);
            InvokeStringStep(actionStep, getParameters);
        }

        private void InvokeStringStep(ActionStepText actionStep, Func<object[]> getParametersForActionStepText)
        {
            if (_actionCatalog.ActionExists(actionStep) == false)
                throw new ArgumentException(string.Format("cannot find Token string '{0}'", actionStep));

            var info = _actionCatalog.GetAction(actionStep);

            Type actionType = GetActionType(info.Action);
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = getParametersForActionStepText();

            BeforeEachScenario(info);
            BeforeEachStep(info);

            methodInfo.Invoke(info.Action, BindingFlags.InvokeMethod, null,
                              new object[] { actionParamValues }, CultureInfo.CurrentCulture);

            info.ExecuteNotificationMethod(typeof(AfterStepAttribute));
            _lastAction = info;
        }

        private void BeforeEachStep(ActionMethodInfo info)
        {
            info.ExecuteNotificationMethod(typeof(BeforeStepAttribute));
        }

        private void BeforeEachScenario(ActionMethodInfo info)
        {
            if (_isFirstStepInScenario)
            {
                _isFirstStepInScenario = false;
                info.ExecuteNotificationMethod(typeof(BeforeScenarioAttribute));
            }
        }

        public void OnCloseScenario()
        {
            if (_lastAction != null)
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
        }

        public ActionStepResult Run(ActionStepText actionStep)
        {
            return Run(actionStep, null);
        }

        public ActionStepResult Run(ActionStepText actionStep, Row row)
        {
            var actionStepToUse = new ActionStepText(actionStep.Step.RemoveFirstWord(), actionStep.FromFile);
            var result = new ActionStepResult(actionStep.Step, new Passed());
            try
            {
                if (_actionCatalog.ActionExists(actionStepToUse) == false)
                {
                    string pendReason = string.Format("No matching Action found for \"{0}\"", actionStep);
                    result = new ActionStepResult(actionStep.Step, new Pending(pendReason));
                }
                else
                {
                    if (row == null)
                        InvokeStringStep(actionStepToUse);
                    else
                        InvokeStringStep(actionStepToUse, row);
                }
            }
            catch (Exception e)
            {
                Exception realException = FindUsefulException(e);
                result = new ActionStepResult(actionStep.Step, new Failed(realException));
            }
            return result;
        }

        private Type GetActionType(object action)
        {
            Type actionType = action.GetType().IsGenericType
                ? action.GetType().GetGenericTypeDefinition()
                : action.GetType();
            return actionType;
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

        public void BeforeScenario()
        {
            _isFirstStepInScenario = true;
        }

        public void AfterScenario()
        {
            if (_lastAction != null)
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
        }
    }
}