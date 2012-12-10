using System.IO;
using NBehave.Configuration;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs
{
    public class SystemTestContext
    {
        protected NBehaveConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            EstablishContext();
            Because();
        }


        protected void Configure_With(params string[] featureFiles)
        {
            _config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] { Path.GetFileName(GetType().Assembly.Location) })
                .SetEventListener(NBehave.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(featureFiles);
        }

        protected virtual void Because()
        {
        }

        protected virtual void EstablishContext()
        {
        }
    }
}