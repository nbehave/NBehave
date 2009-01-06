using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class StoryRunner : RunnerBase
    {
        protected override void ParseAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ThemeAttribute), false).Length > 0)
                {
                    ThemeAttribute themeAttribute = (ThemeAttribute)t.GetCustomAttributes(typeof(ThemeAttribute), false)[0];
                    object themeInstance = Activator.CreateInstance(t);

                    if (StoryRunnerFilter.NamespaceFilter.IsMatch(t.Namespace) &&
                        StoryRunnerFilter.ClassNameFilter.IsMatch(t.Name))
                        Themes.Add(new Pair<string, object>(themeAttribute.Name, themeInstance));
                }
            }
        }

        protected override void RunStories(StoryResults results, IMessageProvider messageProvider, IEventListener listener)
        {
            using (MessageProviderRegistry.RegisterScopedInstance(messageProvider))
            {
                foreach (Pair<string, object> theme in Themes)
                {
                    string themeName = theme.First;
                    object themeClass = theme.Second;

                    listener.ThemeStarted(themeName);

                    MethodInfo[] themeMethods = GetThemeMethods(themeClass);
                    MethodInfo[] storyMethods = GetStoryMethods(themeMethods);

                    foreach (MethodInfo storyMethod in storyMethods)
                    {
                        InvokeStoryMethod(storyMethod, themeClass);
                        CompileStoryResults(results);
                        listener.StoryResults(results);
                        ClearStoryList();
                    }
                    listener.ThemeFinished();
                }
            }
        }

        private static void InvokeStoryMethod(MethodInfo storyMethod, object theme)
        {
            try
            {
                storyMethod.Invoke(theme, null);
            }
            catch { }
        }

        private MethodInfo[] GetStoryMethods(MethodInfo[] themeMethods)
        {
            List<MethodInfo> storyMethods = new List<MethodInfo>();
            foreach (MethodInfo themeMethod in themeMethods)
            {
                if ((themeMethod.GetCustomAttributes(typeof(StoryAttribute), false).Length > 0) &&
                    (StoryRunnerFilter.MethodNameFiler.IsMatch(themeMethod.Name)))
                {
                    storyMethods.Add(themeMethod);
                }
            }
            return storyMethods.ToArray();
        }

        private MethodInfo[] GetThemeMethods(object theme)
        {
            Type themeType = theme.GetType();
            return themeType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private class NoOpMessageProvider : IMessageProvider
        {
            public void AddMessage(string message)
            {
                //do nothing
            }
        }
    }
}