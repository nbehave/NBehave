using System;
using System.Globalization;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class ActionStepRunner
    {
        private readonly ActionCatalog _actionCatalog;

        public ActionStepRunner(ActionCatalog actionCatalog)
        {
            _actionCatalog = actionCatalog;
        }

        public void InvokeTokenString(string tokenString)
        {
            if (_actionCatalog.ActionExists(tokenString) == false)
                throw new ArgumentException(string.Format("cannot find Token string '{0}'", tokenString));

            object action = _actionCatalog.GetAction(tokenString).Action;

            Type actionType = GetActionType(action);
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = _actionCatalog.GetParametersForMessage(tokenString);

            methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                              new object[] { actionParamValues }, CultureInfo.CurrentCulture);
        }

        public Result RunActionStepRow(string row)
        {
            Result result = new Passed();
            try
            {
                string rowWithoutActionType = row.RemoveFirstWord();
                if (_actionCatalog.ActionExists(rowWithoutActionType) == false)
                {
                    string pendReason = string.Format("No matching Action found for \"{0}\"", row);
                    result = new Pending(pendReason);
                }
                else
                    InvokeTokenString(rowWithoutActionType);
            }
            catch (Exception e)
            {
                Exception realException = FindUsefulException(e);
                result = new Failed(realException);
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