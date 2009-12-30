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

        private EventHandler<EventArgs<Feature>> _featureCreatedEventHandler;
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
            _messageAddedEventHandler = (sender, e) => listener.ScenarioMessageAdded(e.EventData.Message);
            StringStep.MessageAdded += _messageAddedEventHandler;
        }

        private void StartWatchingScenarioCreated(IEventListener listener)
        {
            _scenarioCreatedEventHandler = (sender, e) => listener.ScenarioCreated(e.EventData.Title);
            ScenarioWithSteps.ScenarioCreated += _scenarioCreatedEventHandler;
        }

        private void StartWatchingFeatureCreated(IEventListener listener)
        {
            _featureCreatedEventHandler = (sender, e) =>
            {
                e.EventData.IsDryRun = IsDryRun;
                listener.FeatureCreated(e.EventData.Title);
                listener.FeatureNarrative(e.EventData.Narrative);
            };
            Feature.FeatureCreated += _featureCreatedEventHandler;
        }

        private void StopWatching(IEventListener listener)
        {
            Feature.FeatureCreated -= _featureCreatedEventHandler;
            ScenarioWithSteps.ScenarioCreated -= _scenarioCreatedEventHandler;
            StringStep.MessageAdded -= _messageAddedEventHandler;
            ScenarioStepRunner.ScenarioResultCreated -= _scenarioResultAddedEventHandler;
            listener.RunFinished();
        }

        private void InitializeRun(FeatureResults results, IEventListener listener)
        {
            listener.RunStarted();
            results.NumberOfThemes = _themes.Count;
        }
    }
}
