using System;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework.Remoting
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
