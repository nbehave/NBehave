using System.Collections.Generic;
using NBehave.Narrator.Framework.Contracts;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class FeatureProcessor : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;
        private IEnumerable<Feature> _features;
        private bool _actionStepsLoaded;
        private readonly IFeatureRunner _featureRunner;

        public FeatureProcessor(ITinyMessengerHub hub, IFeatureRunner featureRunner)
        {
            _hub = hub;
            _featureRunner = featureRunner;

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

            foreach (Feature feature in _features)
            {
                _hub.Publish(new FeatureStartedEvent(this, feature.Title));
                _hub.Publish(new FeatureNarrativeEvent(this, feature.Narrative));

                _featureRunner.Run(feature);
                _hub.Publish(new FeatureFinishedEvent(this, feature.Title));
            }
        }
    }
}