using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    public class SystemTestContext
    {
        [SetUp]
        public void SetUp()
        {
            EstablishContext();
            Because();
        }

        protected virtual void Because()
        {
        }

        protected virtual void EstablishContext()
        {
        }

        protected NBehaveConfiguration CreateRunnerWithBasicConfiguration()
        {
            var config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] {"TestPlainTextAssembly.dll"})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {TestFeatures.ScenariosWithoutFeature});

            return config;
        }
    }
}