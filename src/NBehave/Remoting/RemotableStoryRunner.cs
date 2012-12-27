using System;
using NBehave.Configuration;
using NBehave.Internal;

namespace NBehave.Remoting
{
    public class RemotableStoryRunner : MarshalByRefObject, IRunner
    {
        private NBehaveConfiguration configuration;

        public void Initialise(NBehaveConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public FeatureResults Run()
        {
            var runner = new TextRunner(configuration);
            var results = runner.Run();
            return results;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
