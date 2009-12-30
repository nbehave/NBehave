using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ParameterConverter
    {
        private readonly ActionCatalog _actionCatalog;

        public ParameterConverter(ActionCatalog actionCatalog)
        {
            _actionCatalog = actionCatalog;
        }

        public object[] GetParametersForActionStepText(ActionStepText actionStepText)
        {
            ActionMethodInfo action = _actionCatalog.GetAction(actionStepText);
            List<string> paramNames = GetParameterNames(action);
            Match match = action.ActionStepMatcher.Match(actionStepText.Step);
            Func<int, string> getValues = i => match.Groups[paramNames[i]].Value;

            return GetParametersForActionStepText(action, paramNames, getValues);
        }

        public object[] GetParametersForActionStepText(ActionStepText actionStepText, Row row)
        {
            ActionMethodInfo action = _actionCatalog.GetAction(actionStepText);
            List<string> paramNames = action.ParameterInfo.Select(a => a.Name).ToList();
            Func<int, string> getValues = i => row.ColumnValues[paramNames[i]];

            return GetParametersForActionStepText(action, paramNames, getValues);
        }

        private object[] GetParametersForActionStepText(ActionMethodInfo action, ICollection<string> paramNames, Func<int, string> getValue)
        {
            ParameterInfo[] args = action.ParameterInfo;
            var values = new object[args.Length];

            for (int argNumber = 0; argNumber < paramNames.Count; argNumber++)
            {
                string strParam = getValue(argNumber);
                values[argNumber] = ChangeParameterType(strParam, args[argNumber]);
            }

            return values;
        }

        private List<string> GetParameterNames(ActionMethodInfo actionValue)
        {
            return actionValue.GetParameterNames();
        }
        private object ChangeParameterType(string strParam, ParameterInfo paramType)
        {
            if (paramType.ParameterType.IsArray)
                return CreateArray(strParam, paramType.ParameterType);
            return Convert.ChangeType(strParam, paramType.ParameterType);
        }

        private object CreateArray(string strParam, Type arrayOfType)
        {
            var strParamAsArray = strParam.Replace(Environment.NewLine, "\n").Split(new[] { ',' });
            for (int i = 0; i < strParamAsArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strParamAsArray[i]) == false)
                    strParamAsArray[i] = strParamAsArray[i].Trim();
            }
            while (string.IsNullOrEmpty(strParamAsArray.Last()))
                strParamAsArray = strParamAsArray.Take(strParamAsArray.Length - 1).ToArray();
            var typedArray = Activator.CreateInstance(arrayOfType, strParamAsArray.Length);
            var typeInList = arrayOfType.GetElementType();
            var method = GetType().GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var types = new[] { typeInList };
            var genMethod = method.MakeGenericMethod(types);
            for (int i = 0; i < strParamAsArray.Length; i++)
            {
                object value = Convert.ChangeType(strParamAsArray[i], typeInList);
                genMethod.Invoke(this, new[] { typedArray, i, value });
            }
            return typedArray;
        }

        //This method is called with reflection by the CreateArray method
        private void SetValue<T>(T[] array, int index, T value)
        {
            array[index] = value;
        }

    }
}