using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public abstract class RunnerBase
    {
        protected abstract void RunFeatures(FeatureResults results);
        protected abstract void ParseAssembly(Assembly assembly);

        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();
        private readonly List<Pair<string, object>> _themes = new List<Pair<string, object>>();

        private EventHandler<EventArgs<Feature>> _storyCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioWithSteps>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<MessageEventData>> _messageAddedEventHandler;
        private EventHandler<EventArgs<ScenarioResult>> _scenarioResultAddedEventHandler;

        public bool IsDryRun { get; set; }
        protected IEventListener EventListener { get; set; }

        protected RunnerBase(IEventListener listener)
        {
            EventListener = listener;
        }

        public FeatureResults Run()
        {
            var results = new FeatureResults();

            try
            {
                InitializeRun(results, EventListener);
                StartWatching(EventListener);
                RunFeatures(results);
            }
            finally
            {
                StopWatching(EventListener);
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

        private void StartWatching(IEventListener listener)
        {
            StartWatchingFeatureCreated(listener);
            StartWatchingFeatureResults(listener);
            StartWatchingScenarioCreated(listener);
            StartWatchingMessageAdded(listener);
        }

        private void StartWatchingFeatureResults(IEventListener listener)
        {
            _scenarioResultAddedEventHandler = (sender, e) => listener.ScenarioResult(e.EventData);
            ScenarioStepRunner.ScenarioResultCreated += _scenarioResultAddedEventHandler;
        }

        private void StartWatchingMessageAdded(IEventListener listener)
        {
            _messageAddedEventHandler = (sender, e) => SelectMessageType(listener, e.EventData);
            StringStep.MessageAdded += _messageAddedEventHandler;
        }

        private void StartWatchingScenarioCreated(IEventListener listener)
        {
            _scenarioCreatedEventHandler = (sender, e) => listener.ScenarioCreated(e.EventData.Title);
            ScenarioWithSteps.ScenarioCreated += _scenarioCreatedEventHandler;
        }

        private void StartWatchingFeatureCreated(IEventListener listener)
        {
            _storyCreatedEventHandler = (sender, e) =>
            {
                e.EventData.IsDryRun = IsDryRun;
                listener.StoryCreated(e.EventData.Title);
                listener.StoryMessageAdded(e.EventData.Narrative);
            };
            Feature.FeatureCreated += _storyCreatedEventHandler;
        }

        private void StopWatching(IEventListener listener)
        {
            Feature.FeatureCreated -= _storyCreatedEventHandler;
            ScenarioWithSteps.ScenarioCreated -= _scenarioCreatedEventHandler;
            StringStep.MessageAdded -= _messageAddedEventHandler;
            ScenarioStepRunner.ScenarioResultCreated -= _scenarioResultAddedEventHandler;
            listener.RunFinished();
        }

        private void SelectMessageType(IEventListener listener, MessageEventData eventData)
        {
            switch (eventData.Type)
            {
                case MessageEventType.StringStep: listener.ScenarioMessageAdded(eventData.Message);
                    break;
                case MessageEventType.Pending: listener.ScenarioMessageAdded(string.Format("Pending: {0}", eventData.Message));
                    break;
                default:
                    listener.StoryMessageAdded(eventData.Message);
                    break;
            }
        }

        private void InitializeRun(FeatureResults results, IEventListener listener)
        {
            listener.RunStarted();
            results.NumberOfThemes = _themes.Count;
        }
    }
}
