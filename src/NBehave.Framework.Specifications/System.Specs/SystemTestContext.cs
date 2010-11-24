using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.IO;

    using NBehave.Narrator.Framework.EventListeners;
    using NBehave.Narrator.Framework.Specifications.Features;

    using NUnit.Framework;

    public class SystemTestContext
    {
        [SetUp]
        public void SetUp()
        {
            this.EstablishContext();
            this.Because();
        }

        protected virtual void Because()
        {
        }

        protected virtual void EstablishContext()
        {
        }

        protected NBehaveConfiguration CreateRunnerWithBasicConfiguration()
        {
            var config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "TestPlainTextAssembly.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { TestFeatures.ScenariosWithoutFeature });

            return config;
        }
    }
}
