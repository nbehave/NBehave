using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionMethodInfo
    {
        public string ActionType { get; private set; } //Given, When Then etc..
        public MethodInfo MethodInfo { get; private set; }
        public Regex ActionStepMatcher { get; private set; }
        public object Action { get; private set; }
        public IFileMatcher FileMatcher { get; set; }
        protected object Instance { get; private set; }

        public ActionMethodInfo()
        {
            FileMatcher = new MatchAllFiles();
        }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo)
            : this(actionStepMatcher, action, methodInfo, string.Empty, null)
        { }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, object instance)
            : this(actionStepMatcher, action, methodInfo, string.Empty, instance)
        { }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType)
            : this(actionStepMatcher, action, methodInfo, actionType, null)
        {
        }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType, object instance)
            : this()
        {
            ActionStepMatcher = actionStepMatcher;
            Action = action;
            MethodInfo = methodInfo;
            ActionType = actionType;
            Instance = instance;
        }

        public ParameterInfo[] ParameterInfo
        {
            get
            {
                return MethodInfo.GetParameters();
            }
        }

        public List<string> GetParameterNames()
        {
            var names = new List<string>();
            int index = 0;
            string name = ".";
            Regex regex = ActionStepMatcher;
            while (string.IsNullOrEmpty(name) == false)
            {
                name = regex.GroupNameFromNumber(index);
                if (string.IsNullOrEmpty(name) == false && name != index.ToString())
                    names.Add(name);
                index++;
            }
            return names;
        }

        public bool MatchesFileName(string fileName)
        {
            return FileMatcher.IsMatch(fileName);
        }

        private MethodInfo LocateNotificationMethod(Type notificationType)
        {
            foreach (var m in MethodInfo.DeclaringType.GetMethods())
            {
                if (m.GetCustomAttributes(notificationType, true).Length > 0)
                    return m;
            }
            return null;
        }

        public void ExecuteNotificationMethod(Type notificationType)
        {
            if (Instance == null)
                return;

            var methodInfo = LocateNotificationMethod(notificationType);
            if (methodInfo == null)
                return;

            methodInfo.Invoke(Instance, new object[0]);
        }
    }
}