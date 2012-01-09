// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterConverter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ParameterConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ParameterConverter
    {
        private readonly ActionCatalog _actionCatalog;
        private readonly TypeConverter typeConverter = new TypeConverter();

        public ParameterConverter(ActionCatalog actionCatalog)
        {
            _actionCatalog = actionCatalog;
        }

        public object[] GetParametersForStep(StringStep stringStep)
        {
            var action = _actionCatalog.GetAction(stringStep);
            var paramNames = GetParameterNames(action);
            var match = action.ActionStepMatcher.Match(stringStep.MatchableStep);
            Func<int, string> getValues = i => match.Groups[paramNames[i]].Value;

            if (IsListStep(action, stringStep))
                return GetParametersForListStep(action, stringStep, paramNames);
            return GetParametersForStep(action, paramNames, getValues);
        }

        private bool IsListStep(ActionMethodInfo action, StringStep step)
        {
            return (action.MethodParametersType == MethodParametersType.TypedListStep
                   || action.MethodParametersType == MethodParametersType.UntypedListStep)
                   && step is StringTableStep;
        }

        public object[] GetParametersForStep(StringStep stringStep, Example example)
        {
            var action = _actionCatalog.GetAction(stringStep);
            var paramNames = action.ParameterInfo.Select(a => a.Name).ToList();
            Func<int, string> getValues = i => example.ColumnValues[paramNames[i]];

            return GetParametersForStep(action, paramNames, getValues);
        }


        private object[] GetParametersForListStep(ActionMethodInfo action, StringStep stringStep, List<string> paramNames)
        {
            var itemType = action.ParameterInfo[0].ParameterType.GetGenericArguments()[0];
            var values = itemType.CreateInstanceOfGenericList();
            var addMethodOnList = values.GetType().GetMethod("Add");
            Func<Example, string> getValues = e => e.ColumnValues[e.ColumnNames[0].Name];
            var method = new ActionMethodInfo(action.ActionStepMatcher, addMethodOnList, addMethodOnList, action.ActionType);
            GetValuesFromTableStep(stringStep, paramNames, getValues, addMethodOnList, values, method);
            return new object[] { values };
        }

        private void GetValuesFromTableStep(StringStep stringStep, List<string> paramNames, Func<Example, string> getValues, MethodInfo addMethodOnList, object values, ActionMethodInfo method)
        {
            foreach (var example in ((StringTableStep)stringStep).TableSteps)
            {
                var value = GetParametersForStep(method, paramNames, _ => getValues(example))[0];
                addMethodOnList.Invoke(values, new[] { value });
            }
        }

        private object[] GetParametersForStep(ActionMethodInfo action, ICollection<string> paramNames, Func<int, string> getValue)
        {
            var args = action.ParameterInfo;
            var values = new object[args.Length];
            if (args.Length == 1 && args[0].ParameterType.IsClass && args[0].ParameterType != typeof(string) 
                && IsArrayOrIEnumerable(args[0])==false)
            {
                values[0] = CreateInstanceOfComplexType(paramNames, args, getValue);
            }
            else
            {
                for (var argNumber = 0; argNumber < paramNames.Count; argNumber++)
                {
                    var strParam = getValue(argNumber);
                    values[argNumber] = typeConverter.ChangeParameterType(strParam, args[argNumber]);
                }
            }
            return values;
        }

        private static bool IsArrayOrIEnumerable(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsArray || parameter.IsGenericIEnumerable();
        }

        private List<string> GetParameterNames(ActionMethodInfo actionValue)
        {
            return actionValue.GetParameterNames();
        }

        private object CreateInstanceOfComplexType(ICollection<string> paramNames, ParameterInfo[] args, Func<int, string> getValue)
        {
            var instance = Activator.CreateInstance(args[0].ParameterType);
            for (var argNumber = 0; argNumber < paramNames.Count; argNumber++)
            {
                var argName = paramNames.ToArray()[argNumber];
                var prop = instance.GetType().GetProperties().FirstOrDefault(_ => _.Name.Equals(argName, StringComparison.CurrentCultureIgnoreCase));
                if (prop == null)
                    throw new ArgumentException(string.Format("Type '{0}' dont have a property with the name '{1}'", instance.GetType().Name, argName));
                var strParam = getValue(argNumber);
                var paramType = prop.GetSetMethod(true).GetParameters()[0];
                var value = typeConverter.ChangeParameterType(strParam, paramType);
                prop.SetValue(instance, value, null);
            }
            return instance;
        }
    }
}