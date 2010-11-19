// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the TextRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TextRunner
    {
        private readonly List<Feature> _features = new List<Feature>();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly IStringStepRunner _stringStepRunner;
        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();
        private EventHandler<EventArgs<Feature>> _featureCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioWithSteps>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioResult>> _scenarioResultAddedEventHandler;

        public TextRunner(IEventListener eventListener)
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _stringStepRunner = new StringStepRunner(ActionCatalog);
            _actionStepFileLoader = new ActionStepFileLoader(_stringStepRunner);
            EventListener = eventListener;
        }

        public StoryRunnerFilter StoryRunnerFilter
        {
            get { return _storyRunnerFilter; }
            set { _storyRunnerFilter = value; }
        }

        public bool IsDryRun { get; set; }

        public ActionCatalog ActionCatalog { get; private set; }

        protected IEventListener EventListener { get; set; }

        public void LoadAssembly(string assemblyPath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyPath));
        }

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

        public void Load(IEnumerable<string> fileLocations)
        {
            _features.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        public void LoadAssembly(Assembly assembly)
        {
            ParseAssembly(assembly);
        }

        protected void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog);
            parser.FindActionSteps(assembly);
        }

        protected void RunFeatures(FeatureResults results)
        {
            EventListener.ThemeStarted(string.Empty);
            RunEachFeature(results, feature => true, scenario => true);
            EventListener.ThemeFinished();
        }

        private void RunEachFeature(FeatureResults featureResults, Func<Feature, bool> featurePredicate, Func<ScenarioWithSteps, bool> scenarioPredicate)
        {
            foreach (var feature in _features.Where(featurePredicate))
            {
                var scenarios = feature.Scenarios.Where(scenarioPredicate);
                var scenarioStepRunner = new ScenarioStepRunner();

                var scenarioResults = scenarioStepRunner.Run(scenarios);
                AddScenarioResultsToStoryResults(scenarioResults, featureResults);
                featureResults.NumberOfStories++;
            }
        }

        private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, FeatureResults featureResults)
        {
            foreach (var result in scenarioResults)
            {
                featureResults.AddResult(result);
            }
        }

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
