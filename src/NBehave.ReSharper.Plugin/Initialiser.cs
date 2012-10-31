using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Internal;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public static class Initializer
    {
        private static bool Initialized;

        public static void Initialize()
        {
            if (Initialized)
                return;
            Initialized = true;
            TinyIoCContainer container = TinyIoCContainer.Current;

            container.Register<IFeatureRunner, FeatureRunner>();
            container.Register<Narrator.Framework.Internal.IFeatureRunner, FeatureRunner>();

            NBehaveConfiguration configuration = CreateConfiguration(typeof(Initializer).Assembly.Location.ToLower(), new List<string>());
            CommonInitializer.Initialise(container, configuration);

        }

        private static NBehaveConfiguration CreateConfiguration(string pathToAssembly, IEnumerable<string> featureFiles)
        {
            return NBehaveConfiguration
                .New
                .SetAssemblies(new[] { pathToAssembly })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(featureFiles);
        }
    }
}