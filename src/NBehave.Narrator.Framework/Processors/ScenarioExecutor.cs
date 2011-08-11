using System.Collections.Generic;
using NBehave.Narrator.Framework.Contracts;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class ScenarioExecutor : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;
        private IEnumerable<Feature> _features;
        private bool _actionStepsLoaded;
        private readonly IScenarioRunner _scenarioRunner;

        public ScenarioExecutor(ITinyMessengerHub hub, IScenarioRunner scenarioRunner)
        {
            _hub = hub;
            _scenarioRunner = scenarioRunner;

            _hub.Subscribe<FeaturesLoaded>(loaded =>
                                               {
                                                   _features = loaded.Content;
                                                   OnRunStarted();
                                               });

            _hub.Subscribe<ActionStepsLoaded>(stepsLoaded =>
                                                  {
                                                      _actionStepsLoaded = true;
                                                      OnRunStarted();
                                                  });
        }

        private void OnRunStarted()
        {
            if (_features == null || !_actionStepsLoaded) return;

            _hub.Publish(new ThemeStartedEvent(this, string.Empty));

            foreach (Feature feature in _features)
            {
                _hub.Publish(new FeatureStartedEvent(this, feature.Title));
                _hub.Publish(new FeatureNarrativeEvent(this, feature.Narrative));

                _scenarioRunner.Run(feature);
                _hub.Publish(new FeatureFinishedEvent(this, feature.Title));
            }

            _hub.Publish(new ThemeFinishedEvent(this));
        }
    }
}