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

            return GetParametersForStep(action, paramNames, getValues);
        }

        public object[] GetParametersForStep(StringStep stringStep, Example example)
        {
            var action = _actionCatalog.GetAction(stringStep);
            var paramNames = action.ParameterInfo.Select(a => a.Name).ToList();
            Func<int, string> getValues = i => example.ColumnValues[paramNames[i]];

            return GetParametersForStep(action, paramNames, getValues);
        }

        private object[] GetParametersForStep(ActionMethodInfo action, ICollection<string> paramNames, Func<int, string> getValue)
        {
            var args = action.ParameterInfo;
            var values = new object[args.Length];
            if (args.Length == 1 && args[0].ParameterType.IsClass && args.Length != paramNames.Count)
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