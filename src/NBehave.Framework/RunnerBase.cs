// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunnerBase.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the RunnerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Reflection;

    public abstract class RunnerBase
    {
        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();

        private EventHandler<EventArgs<Feature>> _featureCreatedEventHandler;

        private EventHandler<EventArgs<ScenarioWithSteps>> _scenarioCreatedEventHandler;

        private EventHandler<EventArgs<ScenarioResult>> _scenarioResultAddedEventHandler;

        protected RunnerBase(IEventListener listener)
        {
            EventListener = listener;
        }

        public bool IsDryRun { get; set; }

        public StoryRunnerFilter StoryRunnerFilter
        {
            get { return _storyRunnerFilter; }
            set { _storyRunnerFilter = value; }
        }

        protected IEventListener EventListener { get; set; }

        public FeatureResults Run()
        {
            var results = new FeatureResults();

            try
            {
                InitializeRun(EventListener);
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

        protected abstract void RunFeatures(FeatureResults results);

        protected abstract void ParseAssembly(Assembly assembly);

        private void StartWatching(IEventListener listener)
        {
            StartWatchingFeatureCreated(listener);
            StartWatchingFeatureResults(listener);
            StartWatchingScenarioCreated(listener);
        }

        private void StartWatchingFeatureResults(IEventListener listener)
        {
            _scenarioResultAddedEventHandler = (sender, e) => listener.ScenarioResult(e.EventData);
            ScenarioStepRunner.ScenarioResultCreated += _scenarioResultAddedEventHandler;
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
            ScenarioStepRunner.ScenarioResultCreated -= _scenarioResultAddedEventHandler;
            listener.RunFinished();
        }

        private void InitializeRun(IEventListener listener)
        {
            listener.RunStarted();
        }
    }
}
