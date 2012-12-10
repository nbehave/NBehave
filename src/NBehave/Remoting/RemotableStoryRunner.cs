using System;
using NBehave.Configuration;
using NBehave.Domain;
using NBehave.Internal;

namespace NBehave.Remoting
{
    public class RemotableStoryRunner : MarshalByRefObject, IRunner
    {
        private NBehaveConfiguration _configuration;

        public void Initialise(NBehaveConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FeatureResults Run()
        {
            var runner = new TextRunner(_configuration);
            var results = runner.Run();
            return results;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
