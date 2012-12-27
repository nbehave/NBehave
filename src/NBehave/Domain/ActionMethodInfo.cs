using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NBehave.Contracts;
using NBehave.Extensions;
using NBehave.Internal;

namespace NBehave
{
    public class ActionMethodInfo
    {
        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType)
            : this(actionStepMatcher, action, methodInfo, actionType, null)
        {
        }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType, object instance)
        {
            ActionStepMatcher = actionStepMatcher;
            Action = action;
            MethodInfo = methodInfo;
            ActionType = actionType;
            Instance = instance;
            FileMatcher = new MatchAllFiles();
            MethodParametersType = FigureOutParameterTypes();
        }

        private MethodParametersType FigureOutParameterTypes()
        {
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length != 1)
                return MethodParametersType.UntypedStep;

            var paramType = parameters.First();
            if (IsStringType(paramType.ParameterType) || paramType.ParameterType.IsPrimitive)
                return MethodParametersType = MethodParametersType.UntypedStep;

            if (paramType.IsGenericIEnumerable())
            {
                var arg = paramType.GetGenericArgument();
                return IsStringType(arg) || arg.IsPrimitive ? MethodParametersType.UntypedListStep : MethodParametersType.TypedListStep;
            }
            return MethodParametersType.TypedStep;
        }

        private static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        public string ActionType { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public Regex ActionStepMatcher { get; private set; }
        public object Action { get; private set; }
        public IFileMatcher FileMatcher { get; set; }

        public ParameterInfo[] ParameterInfo
        {
            get { return MethodInfo.GetParameters(); }
        }

        private object Instance { get; set; }
        public MethodParametersType MethodParametersType { get; private set; }

        public List<string> GetParameterNames()
        {
            var names = new List<string>();
            var index = 0;
            var name = ".";
            var regex = ActionStepMatcher;
            while (string.IsNullOrEmpty(name) == false)
            {
                name = regex.GroupNameFromNumber(index);
                if (string.IsNullOrEmpty(name) == false && name != index.ToString())
                {
                    names.Add(name);
                }

                index++;
            }

            return names;
        }

        public bool MatchesFileName(string fileName)
        {
            return FileMatcher.IsMatch(fileName);
        }

        public void ExecuteNotificationMethod(Type notificationType)
        {
            if (Instance == null)
            {
                return;
            }

            var methodInfo = LocateNotificationMethod(notificationType);
            if (methodInfo == null)
            {
                return;
            }

            methodInfo.Invoke(Instance, new object[0]);
        }

        private MethodInfo LocateNotificationMethod(Type notificationType)
        {
            return MethodInfo.DeclaringType
                .GetMethods()
                .FirstOrDefault(m => m.GetCustomAttributes(notificationType, true).Length > 0);
        }
    }
}