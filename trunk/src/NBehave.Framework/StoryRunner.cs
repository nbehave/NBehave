using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class StoryRunner : RunnerBase
    {
        protected override void ParseAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ThemeAttribute), false).Length > 0 && t.IsAbstract == false)
                {
                    var themeAttribute = (ThemeAttribute)t.GetCustomAttributes(typeof(ThemeAttribute), false)[0];
                    object themeInstance = Activator.CreateInstance(t);

                    if (StoryRunnerFilter.NamespaceFilter.IsMatch(t.Namespace) &&
                        StoryRunnerFilter.ClassNameFilter.IsMatch(t.Name))
                        Themes.Add(new Pair<string, object>(themeAttribute.Name, themeInstance));
                }
            }
        }

        protected override void RunStories(StoryResults results, IEventListener listener)
        {
            foreach (Pair<string, object> theme in Themes)
            {
                string themeName = theme.First;
                object themeClass = theme.Second;

                listener.ThemeStarted(themeName);

                IEnumerable<MethodInfo> themeMethods = GetThemeMethods(themeClass);
                IEnumerable<MethodInfo> storyMethods = GetStoryMethods(themeMethods);

                foreach (MethodInfo storyMethod in storyMethods)
                {
                    InvokeStoryMethod(storyMethod, themeClass, results);
                    CompileStoryResults(results);
                    listener.StoryResults(results);
                    ClearStoryList();
                }
                listener.ThemeFinished();
            }
        }

        private void InvokeStoryMethod(MethodInfo storyMethod, object theme, StoryResults results)
        {
            try
            {
                storyMethod.Invoke(theme, null);
            }
            catch (Exception e)
            {
                var ex = e.InnerException ?? e;
                if (ex.GetType() == typeof(ActionMissingException))
                {
                    Story story = Stories.Last();
                    var thisResult = story.ScenarioResults.Last();
                    if (thisResult.Result.GetType() != typeof(Failed))
                    {
                        thisResult.Pend(ex.Message);
                    }
                }
            }
        }

        private IEnumerable<MethodInfo> GetStoryMethods(IEnumerable<MethodInfo> themeMethods)
        {
            var storyMethods = new List<MethodInfo>();
            foreach (MethodInfo themeMethod in themeMethods)
            {
                if ((themeMethod.GetCustomAttributes(typeof(StoryAttribute), false).Length > 0) &&
                    (StoryRunnerFilter.MethodNameFiler.IsMatch(themeMethod.Name)))
                {
                    storyMethods.Add(themeMethod);
                }
            }
            return storyMethods;
        }

        private IEnumerable<MethodInfo> GetThemeMethods(object theme)
        {
            Type themeType = theme.GetType();
            return themeType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private void CompileStoryResults(StoryResults results)
        {
            foreach (Story storyToProcess in Stories)
            {
                storyToProcess.CompileResults(results);
            }
            results.NumberOfStories += Stories.Count;
        }
    }
}