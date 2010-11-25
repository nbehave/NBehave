using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureResultCollector : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;

        private FeatureResults _featureResults;

        public FeatureResultCollector(ITinyMessengerHub hub)
        {
            _hub = hub;
            this._featureResults = new FeatureResults(this);

            _hub.Subscribe<ScenarioResultMessage>(this.OnScenarioResultRecieved);
            _hub.Subscribe<ThemeFinished>(finished => OnThemeFinished());
        }

        private void OnScenarioResultRecieved(ScenarioResultMessage message)
        {
            _featureResults.AddResult(message.Content);
        }

        private void OnThemeFinished()
        {
            _hub.Publish(_featureResults);
        }
    }
}
