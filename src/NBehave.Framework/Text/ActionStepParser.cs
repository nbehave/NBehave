// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepParser.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ActionStepParser
    {
        private readonly StoryRunnerFilter _storyRunnerFilter;

        private readonly ActionCatalog _actionCatalog;

        public ActionStepParser(StoryRunnerFilter storyRunnerFilter, ActionCatalog actionCatalog)
        {
            _storyRunnerFilter = storyRunnerFilter;
            _actionCatalog = actionCatalog;
        }

        public void FindActionSteps(Assembly assembly)
        {
            foreach (var t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ActionStepsAttribute), true).Length > 0)
                {
                    if (t.IsAbstract == false && _storyRunnerFilter.NamespaceFilter.IsMatch(t.Namespace ?? string.Empty) 
                        && _storyRunnerFilter.ClassNameFilter.IsMatch(t.Name))
                    {
                        FindActionStepMethods(t);
                    }
                }
            }
        }

        public void FindActionStepMethods(Type actionSteps)
        {
            var instance = Activator.CreateInstance(actionSteps);
            FindActionStepMethods(actionSteps, instance);
        }

        public void FindActionStepMethods(Type actionSteps, object instance)
        {
            var methods = GetMethodsWithActionStepAttribute(actionSteps);
            foreach (var method in methods)
            {
                var action = CreateAction(instance, method);
                var m = new ActionMethodInfo(
                    method.ActionStepMatcher, action, method.MethodInfo, method.ActionType, instance);
                AddFileMatcher(m, instance);
                _actionCatalog.Add(m);
            }
        }

        private void AddFileMatcher(ActionMethodInfo action, object instance)
        {
            var fileMatcher = instance as IMatchFiles;
            if (fileMatcher != null)
            {
                action.FileMatcher = fileMatcher.FileMatcher;
            }
            else
            {
                action.FileMatcher = new MatchAllFiles();
            }
        }

        private object CreateAction(object instance, ActionMethodInfo method)
        {
            object action = null;
            var methodInfo = method.MethodInfo;

            switch (CountParameters(method))
            {
                case 0:
                    action = GetActionWithNoParameters(instance, method);
                    break;
                case 1:
                    action = GetActionForOneParameter(instance, methodInfo);
                    break;
                case 2:
                    action = GetActionForTwoParameters(instance, methodInfo);
                    break;
                case 3:
                    action = GetActionForThreeParameters(instance, methodInfo);
                    break;
                case 4:
                    action = GetActionForFourParameters(instance, methodInfo);
                    break;
            }

            return action;
        }

        private object GetActionWithNoParameters(object instance, ActionMethodInfo method)
        {
            Action action = () => method.MethodInfo.Invoke(instance, null);
            return action;
        }

        private object GetActionForOneParameter(object instance, MethodInfo methodInfo)
        {
            Action<object> action = a => methodInfo.Invoke(instance, new[] { ChangeType(methodInfo, a, 0), });
            return action;
        }

        private object GetActionForTwoParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object> action =
                (a, b) =>
                    {
                        var invokeParameters = new[]
                            {
                                this.ChangeType(methodInfo, a, 0), 
                                this.ChangeType(methodInfo, b, 1),
                            };
                        methodInfo.Invoke(instance, invokeParameters);
                    };
            return action;
        }

        private object GetActionForThreeParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object, object> action =
                (a, b, c) =>
                    {
                        var invokeParameters = new[]
                            {
                                this.ChangeType(methodInfo, a, 0), 
                                this.ChangeType(methodInfo, b, 1), 
                                this.ChangeType(methodInfo, c, 2),
                            };
                        methodInfo.Invoke(instance, invokeParameters);
                    };
            return action;
        }

        private object GetActionForFourParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object, object, object> action =
                (a, b, c, d) =>
                    {
                        var invokeParameters = new[]
                            {
                                this.ChangeType(methodInfo, a, 0), 
                                this.ChangeType(methodInfo, b, 1), 
                                this.ChangeType(methodInfo, c, 2),
                                this.ChangeType(methodInfo, d, 3)
                            };
                        methodInfo.Invoke(instance, invokeParameters);
                    };
            return action;
        }

        private IEnumerable<ActionMethodInfo> GetMethodsWithActionStepAttribute(Type actionSteps)
        {
            var methodsWithActionStepAttribute = GetAllMethodsWithActionStepAttribute(actionSteps);
            var allMethods = GetAllMethodsWithActionStepAttribute(methodsWithActionStepAttribute);
            return allMethods;
        }

        private IEnumerable<ActionMethodInfo> GetAllMethodsWithActionStepAttribute(
            IEnumerable<MethodInfo> methodsWithActionStepAttribute)
        {
            var allMethods = new List<ActionMethodInfo>();
            foreach (var method in methodsWithActionStepAttribute)
            {
                foreach (ActionStepAttribute actionStep in method.GetCustomAttributes(typeof(ActionStepAttribute), true))
                {
                    var actionMethodInfo = BuildActionMethodInfo(actionStep, method);
                    allMethods.Add(actionMethodInfo);
                }
            }

            return allMethods;
        }

        private IEnumerable<MethodInfo> GetAllMethodsWithActionStepAttribute(Type actionSteps)
        {
            return
                from method in
                    actionSteps.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where
                    method.GetCustomAttributes(typeof(ActionStepAttribute), true).Length > 0 &&
                    _storyRunnerFilter.MethodNameFiler.IsMatch(method.Name)
                select method;
        }

        private object ChangeType(MethodInfo methodInfo, object parameter, int parameterIndex)
        {
            return Convert.ChangeType(parameter, methodInfo.GetParameters()[parameterIndex].ParameterType);
        }

        private int CountParameters(ActionMethodInfo actionMethodInfo)
        {
            return actionMethodInfo.ParameterInfo.Count();
        }

        private ActionMethodInfo BuildActionMethodInfo(ActionStepAttribute actionStep, MethodInfo method)
        {
            if (actionStep.ActionMatch == null)
            {
                var tokenString = BuildTokenString(method);
                var tokenStringWithoutFirstWord = tokenString.RemoveFirstWord();
                var actionMatcher = tokenStringWithoutFirstWord.AsRegex();
                return new ActionMethodInfo(actionMatcher, null, method, tokenString.GetFirstWord());
            }

            return new ActionMethodInfo(actionStep.ActionMatch, null, method, actionStep.Type);
        }

        private string BuildTokenString(MethodInfo method)
        {
            var methodName = method.Name.Replace('_', ' ');
            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                var paramName = param.Name;
                var pos = methodName.IndexOf(paramName);
                if (pos > 0)
                {
                    methodName = methodName.Substring(0, pos) + "$" + methodName.Substring(pos);
                }
            }

            return methodName;
        }
    }
}