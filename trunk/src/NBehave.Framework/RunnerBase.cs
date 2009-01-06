using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public abstract class RunnerBase
    {
        protected abstract void RunStories(StoryResults results, IMessageProvider messageProvider, IEventListener listener);
        protected abstract void ParseAssembly(Assembly assembly);

        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();
        private readonly List<Pair<string, object>> _themes = new List<Pair<string, object>>();
        private List<Story> _stories = null;
        private EventHandler<EventArgs<Story>> _storyCreatedEventHandler;

        protected List<Pair<string, object>> Themes { get { return _themes; } }
      
        protected List<Story> Stories { get { return _stories; } }

        public bool IsDryRun { get; set; }

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
            _storyCreatedEventHandler = delegate(object sender, EventArgs<Story> e)
            {
                _stories.Add(e.EventData);
                e.EventData.IsDryRun = IsDryRun;
                listener.StoryCreated(e.EventData.Title);
            };
            Story.StoryCreated += _storyCreatedEventHandler;
        }

        protected void StopWatching(IEventListener listener)
        {
            Story.StoryCreated -= _storyCreatedEventHandler;
            listener.RunFinished();
        }

        protected void InitializeRun(StoryResults results, EventMessageProvider messageProvider, IEventListener listener)
        {
            listener.RunStarted();

            results.NumberOfThemes = _themes.Count;
            _stories = new List<Story>();

            messageProvider.MessageAdded +=
                delegate(object sender, EventArgs<string> e) { listener.StoryMessageAdded(e.EventData); };
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
