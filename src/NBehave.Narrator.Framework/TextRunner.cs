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

            var results = new FeatureResults();
            _hub.Subscribe<FeatureResultEvent>(_ => results.Add(_.Content));

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