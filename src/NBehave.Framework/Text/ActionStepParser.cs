using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
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
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ActionStepsAttribute), true).Length > 0)
                {
                    if (t.IsAbstract == false
                        && _storyRunnerFilter.NamespaceFilter.IsMatch(t.Namespace ?? "")
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
            foreach (ActionMethodInfo method in methods)
            {
                object action = CreateAction(instance, method);
                var m = new ActionMethodInfo(method.ActionStepMatcher, action, method.MethodInfo, method.ActionType, instance);
                AddFileMatcher(m, instance);
                _actionCatalog.Add(m);
            }
        }

        private void AddFileMatcher(ActionMethodInfo action, object instance)
        {
            var fileMatcher = instance as IMatchFiles;
            if (fileMatcher != null)
                action.FileMatcher = fileMatcher.FileMatcher;
            else
                action.FileMatcher = new MatchAllFiles();
        }

        private object CreateAction(object instance, ActionMethodInfo method)
        {
            object action = null;
            MethodInfo methodInfo = method.MethodInfo;

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
                case 5:
                    action = GetActionForFiveParameters(instance, methodInfo);
                    break;
                case 6:
                    action = GetActionForSixParameters(instance, methodInfo);
                    break;
                case 7:
                    action = GetActionForSevenParameters(instance, methodInfo);
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
            Action<object> action = a =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo, a, 0),
                                  });
            return action;
        }

        private object GetActionForTwoParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object> action = (a, b) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                  });
            return action;
        }

        private object GetActionForThreeParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object, object> action = (a, b, c) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                    ChangeType(methodInfo,c,2),
                                  });
            return action;
        }

        private object GetActionForFourParameters(object instance, MethodInfo methodInfo)
        {
            Action<object, object, object, object> action = (a, b, c, d) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                    ChangeType(methodInfo,c,2),
                                    ChangeType(methodInfo,d,3)
                                  });
            return action;
        }

        public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
        private object GetActionForFiveParameters(object instance, MethodInfo methodInfo)
        {
            ActionStepParser.Action<object, object, object, object, object> action = (a, b, c, d, e) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                    ChangeType(methodInfo,c,2),
                                    ChangeType(methodInfo,d,3),
                                    ChangeType(methodInfo,e,4),
                                  });
            return action;
        }

        public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
        private object GetActionForSixParameters(object instance, MethodInfo methodInfo)
        {
            ActionStepParser.Action<object, object, object, object, object, object> action = (a, b, c, d, e, f) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                    ChangeType(methodInfo,c,2),
                                    ChangeType(methodInfo,d,3),
                                    ChangeType(methodInfo,e,4),
                                    ChangeType(methodInfo,f,5),
                                  });
            return action;
        }

        public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
        private object GetActionForSevenParameters(object instance, MethodInfo methodInfo)
        {
            ActionStepParser.Action<object, object, object, object, object, object, object> action = (a, b, c, d, e, f, g) =>
                methodInfo.Invoke(instance, new[]
                                  {
                                    ChangeType(methodInfo,a,0),
                                    ChangeType(methodInfo,b,1),
                                    ChangeType(methodInfo,c,2),
                                    ChangeType(methodInfo,d,3),
                                    ChangeType(methodInfo,e,4),
                                    ChangeType(methodInfo,f,5),
                                    ChangeType(methodInfo,g,6)
                                  });
            return action;
        }

        private IEnumerable<ActionMethodInfo> GetMethodsWithActionStepAttribute(Type actionSteps)
        {
            IEnumerable<MethodInfo> methodsWithActionStepAttribute = GetAllMethodsWithActionStepAttribute(actionSteps);
            List<ActionMethodInfo> allMethods = GetAllMethodsWithActionStepAttribute(methodsWithActionStepAttribute);
            return allMethods;
        }

        private List<ActionMethodInfo> GetAllMethodsWithActionStepAttribute(IEnumerable<MethodInfo> methodsWithActionStepAttribute)
        {
            var allMethods = new List<ActionMethodInfo>();
            foreach (var method in methodsWithActionStepAttribute)
            {
                foreach (ActionStepAttribute actionStep in method.GetCustomAttributes(typeof(ActionStepAttribute), true))
                {
                    ActionMethodInfo actionMethodInfo = BuildActionMethodInfo(actionStep, method);
                    allMethods.Add(actionMethodInfo);
                }
            }
            return allMethods;
        }

        private IEnumerable<MethodInfo> GetAllMethodsWithActionStepAttribute(Type actionSteps)
        {
            return from method in
                       actionSteps.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where
                   method.GetCustomAttributes(typeof(ActionStepAttribute), true).Length > 0
                   && _storyRunnerFilter.MethodNameFiler.IsMatch(method.Name)
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
                string tokenString = BuildTokenString(method);
                string tokenStringWithoutFirstWord = tokenString.RemoveFirstWord();
                Regex actionMatcher = tokenStringWithoutFirstWord.AsRegex();
                return new ActionMethodInfo(actionMatcher, null, method, tokenString.GetFirstWord());
            }

            return new ActionMethodInfo(actionStep.ActionMatch, null, method, actionStep.Type);
        }

        private string BuildTokenString(MethodInfo method)
        {
            string methodName = method.Name.Replace('_', ' ');
            ParameterInfo[] parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                var paramName = param.Name;
                int pos = methodName.IndexOf(paramName);
                if (pos > 0)
                    methodName = methodName.Substring(0, pos) + "$" + methodName.Substring(pos);
            }
            return methodName;
        }
    }
}
