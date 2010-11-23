using System;
using System.Globalization;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class ActionStepRunner
    {
        private readonly ActionCatalog _actionCatalog;
        private bool _isFirstStep = true;
        private ActionMethodInfo _lastAction;

        public ActionStepRunner(ActionCatalog actionCatalog)
        {
            _actionCatalog = actionCatalog;
        }

        public void InvokeTokenString(ActionStepText actionStep)
        {
            if (_actionCatalog.ActionExists(actionStep) == false)
                throw new ArgumentException(string.Format("cannot find Token string '{0}'", actionStep));

            var info = _actionCatalog.GetAction(actionStep);

            Type actionType = GetActionType(info.Action);
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = _actionCatalog.GetParametersForActionStepText(actionStep);

            if(_isFirstStep)
            {
                _isFirstStep = false;
                info.ExecuteNotificationMethod(typeof (BeforeScenarioAttribute));
            }

            info.ExecuteNotificationMethod(typeof(BeforeStepAttribute));

            methodInfo.Invoke(info.Action, BindingFlags.InvokeMethod, null,
                              new object[] { actionParamValues }, CultureInfo.CurrentCulture);

            info.ExecuteNotificationMethod(typeof(AfterStepAttribute));
            _lastAction = info;
        }

        public void OnCloseScenario()
        {
            if(_lastAction != null)
                _lastAction.ExecuteNotificationMethod(typeof(AfterScenarioAttribute));
        }

        public ActionStepResult RunActionStepRow(ActionStepText actionStep)
        {
        	var actionStepToUse = new ActionStepText(actionStep.Text.RemoveFirstWord(),actionStep.FromFile);
        	ActionStepResult result = new ActionStepResult(actionStep.Text, new Passed());
            try
            {
                if (_actionCatalog.ActionExists(actionStepToUse) == false)
                {
                    string pendReason = string.Format("No matching Action found for \"{0}\"", actionStep);
                    result = new ActionStepResult(actionStep.Text, new Pending(pendReason));
                }
                else
                    InvokeTokenString(actionStepToUse);
            }
            catch (Exception e)
            {
                Exception realException = FindUsefulException(e);
                result = new ActionStepResult(actionStep.Text, new Failed(realException));
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
    }
}