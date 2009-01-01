using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class StoryRunner
    {
        private readonly List<Pair<string, object>> _themes = new List<Pair<string, object>>();
        private List<Story> _stories;
        private EventHandler<EventArgs<Story>> _storyCreatedEventHandler;
        private EventHandler<EventArgs<Scenario>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<MessageEventData>> _messageAddedEventHandler;

        public StoryRunner()
        {
            StoryRunnerFilter = new StoryRunnerFilter();
        }

        protected List<Story> Stories { get { return _stories; } }

        public bool IsDryRun { get; set; }

        public StoryRunnerFilter StoryRunnerFilter { get; set; }

        public void LoadAssembly(string assemblyPath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyPath));
        }

        public void LoadAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(ThemeAttribute), false).Length > 0)
                {
                    var themeAttribute = (ThemeAttribute)t.GetCustomAttributes(typeof(ThemeAttribute), false)[0];
                    object themeInstance = Activator.CreateInstance(t);

                    if (StoryRunnerFilter.NamespaceFilter.IsMatch(t.Namespace) &&
                        StoryRunnerFilter.ClassNameFilter.IsMatch(t.Name))
                        _themes.Add(new Pair<string, object>(themeAttribute.Name, themeInstance));
                }
            }
        }

        public StoryResults Run(IEventListener listener)
        {
            var results = new StoryResults();

            try
            {
                InitializeRun(results, listener);
                StartWatching(listener);
                RunStories(results, listener);
            }
            finally
            {
                StopWatching(listener);
            }

            return results;
        }

        private void RunStories(StoryResults results, IEventListener listener)
        {
            //using (MessageProviderRegistry.RegisterScopedInstance(messageProvider))
            //{
            foreach (Pair<string, object> theme in _themes)
            {
                string themeName = theme.First;
                object themeClass = theme.Second;

                listener.ThemeStarted(themeName);

                IEnumerable<MethodInfo> themeMethods = GetThemeMethods(themeClass);
                IEnumerable<MethodInfo> storyMethods = GetStoryMethods(themeMethods);

                foreach (MethodInfo storyMethod in storyMethods)
                {
                    InvokeStoryMethod(storyMethod, themeClass);
                    CompileStoryResults(results);
                    listener.StoryResults(results);
                    ClearStoryList();
                }
                listener.ThemeFinished();
            }
            //}
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
            catch { }
        }

        private void StartWatching(IEventListener listener)
        {
            StartWatchingStoryCreated(listener);
            StartWatchingScenarioCreated(listener);
            StartWatchingMessageAdded(listener);
        }

        private void StartWatchingMessageAdded(IEventListener listener)
        {
            _messageAddedEventHandler = (sender, e) => SelectMessageType(listener, sender, e.EventData);
            Story.MessageAdded += _messageAddedEventHandler;
        }

        private void SelectMessageType(IEventListener listener, object sender, MessageEventData eventData)
        {
            Debug.WriteLine(string.Format("Message Added: {0} :: {1}",
                                          eventData.Type, eventData.Message));
            switch (eventData.Type)
            {
                case "Given":
                case "When":
                case "Then":
                case "And": listener.ScenarioMessageAdded(eventData.Message);
                    break;
                default:
                    listener.StoryMessageAdded(eventData.Message);
                    break;
            }

        }

        private void StartWatchingScenarioCreated(IEventListener listener)
        {
            _scenarioCreatedEventHandler = (sender, e) => listener.ScenarioCreated(e.EventData.Title);
            Story.ScenarioCreated += _scenarioCreatedEventHandler;
        }

        private void StartWatchingStoryCreated(IEventListener listener)
        {
            _storyCreatedEventHandler = (sender, e) =>
                                            {
                                                _stories.Add(e.EventData);
                                                e.EventData.IsDryRun = IsDryRun;
                                                listener.StoryCreated(e.EventData.Title);
                                            };
            Story.StoryCreated += _storyCreatedEventHandler;
        }

        private void StopWatching(IEventListener listener)
        {
            Story.StoryCreated -= _storyCreatedEventHandler;
            Story.ScenarioCreated -= _scenarioCreatedEventHandler;
            Story.MessageAdded -= _messageAddedEventHandler;
            listener.RunFinished();
        }

        private void InitializeRun(StoryResults results, IEventListener listener)
        {
            listener.RunStarted();
            results.NumberOfThemes = _themes.Count;
            _stories = new List<Story>();
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
    }
}