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
        private readonly NBehaveConfiguration _configuration;

        private readonly List<Feature> _features = new List<Feature>();
        private readonly ActionStepFileLoader _actionStepFileLoader;
        private readonly IStringStepRunner _stringStepRunner;
        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter();
        private EventHandler<EventArgs<Feature>> _featureCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioWithSteps>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioResult>> _scenarioResultAddedEventHandler;

        public TextRunner(NBehaveConfiguration configuration)
        {
            _configuration = configuration;
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _stringStepRunner = new StringStepRunner(ActionCatalog);
            _actionStepFileLoader = new ActionStepFileLoader(_stringStepRunner);
        }

        public StoryRunnerFilter StoryRunnerFilter
        {
            get { return _storyRunnerFilter; }
            set { _storyRunnerFilter = value; }
        }

        public ActionCatalog ActionCatalog { get; private set; }

        public FeatureResults Run()
        {
            foreach (var assembly in _configuration.Assemblies)
            {
                LoadAssembly(assembly);    
            }

            this.Load(_configuration.ScenarioFiles);

            var results = new FeatureResults();

            try
            {
                InitializeRun();
                StartWatching();
                RunFeatures(results);
            }
            finally
            {
                StopWatching();
            }

            return results;
        }

        private void LoadAssembly(string assemblyPath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyPath));
        }

        private void Load(IEnumerable<string> fileLocations)
        {
            _features.AddRange(_actionStepFileLoader.Load(fileLocations));
        }

        private void LoadAssembly(Assembly assembly)
        {
            ParseAssembly(assembly);
        }

        private void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog);
            parser.FindActionSteps(assembly);
        }

        private void RunFeatures(FeatureResults results)
        {
            _configuration.EventListener.ThemeStarted(string.Empty);
            RunEachFeature(results, feature => true, scenario => true);
            _configuration.EventListener.ThemeFinished();
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

        private void StartWatching()
        {
            StartWatchingFeatureCreated(_configuration.EventListener);
            StartWatchingFeatureResults(_configuration.EventListener);
            StartWatchingScenarioCreated(_configuration.EventListener);
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
                e.EventData.IsDryRun = _configuration.IsDryRun;
                listener.FeatureCreated(e.EventData.Title);
                listener.FeatureNarrative(e.EventData.Narrative);
            };
            Feature.FeatureCreated += _featureCreatedEventHandler;
        }

        private void StopWatching()
        {
            Feature.FeatureCreated -= _featureCreatedEventHandler;
            ScenarioWithSteps.ScenarioCreated -= _scenarioCreatedEventHandler;
            ScenarioStepRunner.ScenarioResultCreated -= _scenarioResultAddedEventHandler;
            _configuration.EventListener.RunFinished();
        }

        private void InitializeRun()
        {
            _configuration.EventListener.RunStarted();
        }
    }
}
