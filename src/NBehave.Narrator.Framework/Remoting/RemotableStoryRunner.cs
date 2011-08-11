using System;

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
            return new TextRunner(_configuration).Run();
        }
    }
}
