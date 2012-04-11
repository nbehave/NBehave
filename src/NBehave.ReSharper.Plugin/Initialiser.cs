using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Internal;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public static class Initialiser
    {
        private static bool _initialized;

        public static void Initialise()
        {
            if (_initialized)
                return;
            _initialized = true;
            TinyIoCContainer container = TinyIoCContainer.Current;

            container.Register<IFeatureRunner, FeatureRunner>();
            container.Register<Narrator.Framework.Internal.IFeatureRunner, FeatureRunner>();

            NBehaveConfiguration configuration = CreateConfiguration(typeof(Initialiser).Assembly.Location.ToLower(), new List<string>());
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