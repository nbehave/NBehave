using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public abstract class RunnerBase
    {
        protected abstract void RunStories(StoryResults results, IEventListener listener);
        protected abstract void ParseAssembly(Assembly assembly);

        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();
        private readonly List<Pair<string, object>> _themes = new List<Pair<string, object>>();
        private List<Story> _stories;

        private EventHandler<EventArgs<Story>> _storyCreatedEventHandler;
        private EventHandler<EventArgs<Scenario>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<MessageEventData>> _messageAddedEventHandler;

        protected List<Pair<string, object>> Themes { get { return _themes; } }

        protected List<Story> Stories { get { return _stories; } }

        public bool IsDryRun { get; set; }

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

        public void LoadAssembly(string assemblyPath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyPath));
        }

        public void LoadAssembly(Assembly assembly)
        {
            ParseAssembly(assembly);
        }

        public StoryRunnerFilter StoryRunnerFilter
        {
            get { return _storyRunnerFilter; }
            set { _storyRunnerFilter = value; }
        }

        protected void StartWatching(IEventListener listener)
        {
            StartWatchingStoryCreated(listener);
            StartWatchingScenarioCreated(listener);
            StartWatchingMessageAdded(listener);
        }

        private void StartWatchingMessageAdded(IEventListener listener)
        {
            _messageAddedEventHandler = (sender, e) => SelectMessageType(listener, e.EventData);
            Story.MessageAdded += _messageAddedEventHandler;
        }

        private void SelectMessageType(IEventListener listener, MessageEventData eventData)
        {
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

        protected void InitializeRun(StoryResults results, IEventListener listener)
        {
            listener.RunStarted();
            results.NumberOfThemes = _themes.Count;
            _stories = new List<Story>();
        }

        protected void ClearStoryList()
        {
            _stories.Clear();
        }

        protected void CompileStoryResults(StoryResults results)
        {
            foreach (Story storyToProcess in _stories)
            {
                storyToProcess.CompileResults(results);
            }
            results.NumberOfStories += _stories.Count;
        }
    }
}
