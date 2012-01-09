using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public static class ReflectionExtensions
    {
        public static bool IsGenericIEnumerable(this ParameterInfo paramType)
        {
            if (paramType.ParameterType.IsGenericType == false)
            {
                return false;
            }

            var genericArgument = paramType.GetGenericArgument();

            var ien = typeof(List<>).CreateGenericInstance(genericArgument);
            return paramType.ParameterType.IsAssignableFrom(ien.GetType());
        }

        public static Type GetGenericArgument(this ParameterInfo paramType)
        {
            var genericArgs = paramType.ParameterType.GetGenericArguments();
            if (genericArgs.Length > 1)
            {
                throw new NotSupportedException("Sorry, nbehave only supports one generic parameter");
            }
            return genericArgs[0];
        }

        public static object CreateInstanceOfGenericList(this Type parameterType)
        {
            return typeof(List<>).CreateGenericInstance(parameterType);
        }

        public static object CreateGenericInstance(this Type generic, Type innerType)
        {
            var specificType = generic.MakeGenericType(new[] { innerType });
            return Activator.CreateInstance(specificType, null);
        }
    }
}