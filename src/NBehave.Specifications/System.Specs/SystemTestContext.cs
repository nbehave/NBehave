using System.IO;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
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
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
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