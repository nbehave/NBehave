using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class MethodWithAttributeFinder
    {
        private readonly StoryRunnerFilter storyRunnerFilter;

        public MethodWithAttributeFinder(StoryRunnerFilter storyRunnerFilter)
        {
            this.storyRunnerFilter = storyRunnerFilter;
        }

        public IEnumerable<T> FindMethodsWithAttribute<T, TAttrib>(Type actionSteps, Func<TAttrib, MethodInfo, T> buildReturnType)
        {
            var methodsWithActionStepAttribute = FindAllMethodsWithAttribute(actionSteps, typeof(TAttrib));
            var allMethods = FindAllMethodsWithAttribute<T, TAttrib>(methodsWithActionStepAttribute, buildReturnType);
            return allMethods;
        }

        private IEnumerable<T> FindAllMethodsWithAttribute<T, TAttrib>(IEnumerable<MethodInfo> methodsWithActionStepAttribute, Func<TAttrib, MethodInfo, T> buildReturnType)
        {
            var allMethods = new List<T>();
            foreach (var method in methodsWithActionStepAttribute)
            {
                foreach (TAttrib actionStep in method.GetCustomAttributes(typeof(TAttrib), true))
                {
                    var actionMethodInfo = buildReturnType(actionStep, method);
                    allMethods.Add(actionMethodInfo);
                }
            }

            return allMethods;
        }

        private IEnumerable<MethodInfo> FindAllMethodsWithAttribute(Type actionSteps, Type attributeToFind)
        {
            return
                from method in
                    actionSteps.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where
                    method.GetCustomAttributes(attributeToFind, true).Length > 0 &&
                    storyRunnerFilter.MethodNameFiler.IsMatch(method.Name)
                select method;
        }
    }
}