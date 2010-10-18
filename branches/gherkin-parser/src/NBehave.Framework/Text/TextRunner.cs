using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
	public class TextRunner : RunnerBase
	{
		private readonly List<Feature> _features = new List<Feature>();
		private readonly ActionStepFileLoader _actionStepFileLoader;
		private readonly IStringStepRunner _stringStepRunner;
		
		public ActionCatalog ActionCatalog { get; private set; }

		public TextRunner(IEventListener eventListener) : base(eventListener)
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
			    var scenarioStepRunner = new ScenarioStepRunner();

				var scenarioResults = scenarioStepRunner.Run(scenarios);
				AddScenarioResultsToStoryResults(scenarioResults, featureResults);
				featureResults.NumberOfStories++;
			}
		}

		private void AddScenarioResultsToStoryResults(IEnumerable<ScenarioResult> scenarioResults, FeatureResults featureResults)
		{
			foreach (var result in scenarioResults)
				featureResults.AddResult(result);
		}

	    public void Load(IEnumerable<string> fileLocations)
		{
			_features.AddRange(_actionStepFileLoader.Load(fileLocations));
		}
	}
}
