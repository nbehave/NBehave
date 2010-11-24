using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NBehave.Narrator.Framework
{
	public class TextRunner : RunnerBase
	{
		private readonly List<Feature> _features = new List<Feature>();
		private readonly ActionStepFileLoader _actionStepFileLoader;
		private readonly IStringStepRunner _stringStepRunner;
		
		public ActionCatalog ActionCatalog { get; private set; }

		public TextRunner(IEventListener eventListener)
			: base(eventListener)
		{
			ActionCatalog = new ActionCatalog();
			StoryRunnerFilter = new StoryRunnerFilter();
			_stringStepRunner = new StringStepRunner(ActionCatalog);
			_actionStepFileLoader = new ActionStepFileLoader(_stringStepRunner);
		}

		protected override void ParseAssembly(Assembly assembly)
		{
			var parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog);
			parser.FindActionSteps(assembly);
		}

		protected override void RunScenario(FeatureResults results, string featureTitle, string scenarioTitle)
		{
			EventListener.ThemeStarted(string.Empty);
			RunEachFeature(results, feature => String.Compare(feature.Title.Trim(), featureTitle.Trim(), true)==0, scenario => String.Compare(scenario.Title.Trim(), scenarioTitle.Trim(), true)==0);
			EventListener.ThemeFinished();
		}
		
		protected override void RunFeatures(FeatureResults results)
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
				ScenarioStepRunner scenarioStepRunner = CreateScenarioStepRunner();

				IEnumerable<ScenarioResult> scenarioResults = scenarioStepRunner.Run(scenarios);
				AddScenarioResultsToStoryResults(scenarioResults, featureResults);
				featureResults.NumberOfStories++;
				//EventListener.StoryResults(featureResults);
			}
		}

		private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, FeatureResults featureResults)
		{
			foreach (var result in scenarioResults)
				featureResults.AddResult(result);
		}

		private ScenarioStepRunner CreateScenarioStepRunner()
		{
			var scenarioStepRunner = new ScenarioStepRunner(_stringStepRunner);
			return scenarioStepRunner;
		}

		public void Load(IEnumerable<string> fileLocations)
		{
			_features.AddRange(_actionStepFileLoader.Load(fileLocations));
		}

		public void Load(Stream stream)
		{
			var features = _actionStepFileLoader.Load(stream);
			_features.AddRange(features);
		}
	}
}
