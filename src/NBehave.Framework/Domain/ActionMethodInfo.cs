// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionMethodInfo.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionMethodInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public class ActionMethodInfo
    {
        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType)
            : this(actionStepMatcher, action, methodInfo, actionType, null)
        {
        }

        public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType, object instance)
            : this()
        {
            this.ActionStepMatcher = actionStepMatcher;
            this.Action = action;
            this.MethodInfo = methodInfo;
            this.ActionType = actionType;
            this.Instance = instance;
        }

        private ActionMethodInfo()
        {
            this.FileMatcher = new MatchAllFiles();
        }

        public string ActionType { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        public Regex ActionStepMatcher { get; private set; }

        public object Action { get; private set; }

        public IFileMatcher FileMatcher { get; set; }

        public ParameterInfo[] ParameterInfo
        {
            get
            {
                return this.MethodInfo.GetParameters();
            }
        }

        private object Instance { get; set; }

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
            if (this.Instance == null)
            {
                return;
            }

            var methodInfo = this.LocateNotificationMethod(notificationType);
            if (methodInfo == null)
            {
                return;
            }

            methodInfo.Invoke(this.Instance, new object[0]);
        }

        private MethodInfo LocateNotificationMethod(Type notificationType)
        {
            return MethodInfo.DeclaringType
                    .GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttributes(notificationType, true).Length > 0);
        }
    }
}