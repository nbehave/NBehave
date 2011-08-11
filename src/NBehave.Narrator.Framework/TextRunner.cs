using System;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class TextRunner : MarshalByRefObject, IRunner
    {
        private readonly NBehaveConfiguration _configuration;
        private ITinyMessengerHub _hub;

        public TextRunner(NBehaveConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FeatureResults Run()
        {
            NBehaveInitialiser.Initialise(_configuration);
            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();

            FeatureResults results = null;
            _hub.Subscribe<FeatureResults>(featureResults => results = featureResults);

            try
            {
                _hub.Publish(new RunStartedEvent(this));
            }
            finally
            {
                _hub.Publish(new RunFinishedEvent(this));
            }

            return results;
        }
    }
}