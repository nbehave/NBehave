using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class StoryRunner
    {
        private List<Pair<string, object>> _themes = new List<Pair<string, object>>();
        private List<Story> _stories = null;
        private EventHandler<EventArgs<Story>> _storyCreatedEventHandler;
        private bool _isDryRun;

        public bool IsDryRun
        {
            get { return _isDryRun; }
            set { _isDryRun = value; }
        }

        public void LoadAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ThemeAttribute), false).Length > 0)
                {
                    ThemeAttribute themeAttribute = (ThemeAttribute) t.GetCustomAttributes(typeof (ThemeAttribute), false)[0];
                    object themeInstance = Activator.CreateInstance(t);

                    _themes.Add(new Pair<string, object>(themeAttribute.Name, themeInstance));
                }
            }
        }

        public StoryResults Run(IEventListener listener)
        {
            StoryResults results = new StoryResults();
            EventMessageProvider messageProvider = new EventMessageProvider();

            try
            {
                InitializeRun(results, messageProvider, listener);

                StartWatching(listener);

                RunStories(results, messageProvider, listener);
            }
            finally
            {
                StopWatching(listener);
            }

            return results;
        }

        private void RunStories(StoryResults results, IMessageProvider messageProvider, IEventListener listener)
        {
            using (MessageProviderRegistry.RegisterScopedInstance(messageProvider))
            {
                foreach (Pair<string, object> theme in _themes)
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

                        ClearStoryList();
                    }
                    
                    listener.ThemeFinished();
                }
            }
        }

        private void ClearStoryList()
        {
            _stories.Clear();
        }

        private void CompileStoryResults(StoryResults results)
        {
            foreach (Story storyToProcess in _stories)
            {
                storyToProcess.CompileResults(results);
            }
            results.NumberOfStories += _stories.Count;
        }

        private static void InvokeStoryMethod(MethodInfo storyMethod, object theme)
        {
            try
            {
                storyMethod.Invoke(theme, null);
            }
            catch {}
        }

        private void StartWatching(IEventListener listener)
        {
            _storyCreatedEventHandler = delegate(object sender, EventArgs<Story> e)
                                            {
                                                _stories.Add(e.EventData);
                                                e.EventData.IsDryRun = IsDryRun;
                                                listener.StoryCreated();
                                            };
            Story.StoryCreated += _storyCreatedEventHandler;
        }

        private void StopWatching(IEventListener listener)
        {
            Story.StoryCreated -= _storyCreatedEventHandler;
            listener.RunFinished();
        }

        private void InitializeRun(StoryResults results, EventMessageProvider messageProvider, IEventListener listener)
        {
            listener.RunStarted();

            results.NumberOfThemes = _themes.Count;
            _stories = new List<Story>();
            
            messageProvider.MessageAdded +=
                delegate(object sender, EventArgs<string> e) { listener.StoryMessageAdded(e.EventData); };
        }

        private MethodInfo[] GetStoryMethods(MethodInfo[] themeMethods)
        {
            List<MethodInfo> storyMethods = new List<MethodInfo>();
            foreach (MethodInfo themeMethod in themeMethods)
            {
                if (themeMethod.GetCustomAttributes(typeof(StoryAttribute), false).Length > 0)
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