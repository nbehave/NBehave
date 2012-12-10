using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Extensions;

namespace NBehave.Internal
{
    public class TypeConverter
    {
        public object ChangeParameterType(string strParam, ParameterInfo paramType)
        {
            var me = new TypeConverter();
            if (paramType.ParameterType.IsArray)
                return me.CreateArray(strParam, paramType.ParameterType);

            if (paramType.IsGenericIEnumerable())
                return me.CreateList(strParam, paramType.ParameterType);

            if (paramType.ParameterType.IsEnum)
                return Enum.Parse(paramType.ParameterType, strParam);
            
            return Convert.ChangeType(strParam, paramType.ParameterType);
        }

        private object CreateArray(string strParam, Type arrayOfType)
        {
            var strParamAsArray = GetParamAsArray(strParam);
            var typedArray = Activator.CreateInstance(arrayOfType, strParamAsArray.Length);
            var typeInList = arrayOfType.GetElementType();
            SetValues(strParamAsArray, typeInList, typedArray, "SetValue");
            return typedArray;
        }

        public object CreateList(string param, Type parameterType)
        {
            var innerType = parameterType.GetGenericArguments()[0];
            var genericList = innerType.CreateInstanceOfGenericList();
            var strParamAsArray = GetParamAsArray(param);
            SetValues(strParamAsArray, innerType, genericList, "AddValue");
            return genericList;
        }

        private void SetValues(string[] strParamAsArray, Type typeInList, object typedArray, string function)
        {
            var method = GetType().GetMethod(function, BindingFlags.NonPublic | BindingFlags.Instance);
            var types = new[] { typeInList };
            var genMethod = method.MakeGenericMethod(types);
            for (var i = 0; i < strParamAsArray.Length; i++)
            {
                var value = Convert.ChangeType(strParamAsArray[i], typeInList);
                genMethod.Invoke(this, new[] { typedArray, i, value });
            }
        }

        private string[] GetParamAsArray(string strParam)
        {
            var strParamAsArray = strParam.Replace(Environment.NewLine, "\n").Split(new[] { ',' });
            TrimValues(strParamAsArray);
            var trimmedArray = TrimEnd(strParamAsArray);
            return trimmedArray;
        }

        private void TrimValues(string[] strParamAsArray)
        {
            for (var i = 0; i < strParamAsArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strParamAsArray[i]) == false)
                {
                    strParamAsArray[i] = strParamAsArray[i].Trim();
                }
            }
        }

        private string[] TrimEnd(string[] strParamAsArray)
        {
            while (string.IsNullOrEmpty(strParamAsArray.Last()))
            {
                strParamAsArray = strParamAsArray.Take(strParamAsArray.Length - 1).ToArray();
            }

            return strParamAsArray;
        }
        // This method is called with reflection by the CreateArray method
        private void SetValue<T>(T[] array, int index, T value)
        {
            array[index] = value;
        }

        // This method is called with reflection by the CreateArray method
        private void AddValue<T>(ICollection<T> array, int index, T value)
        {
            array.Add(value);
        }
    }
}