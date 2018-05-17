using System.Reflection;
using NBehave.Configuration;

namespace NBehave.Hooks
{
    public class LoadHooks
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly HooksCatalog _hooks;

        public LoadHooks(NBehaveConfiguration configuration, HooksCatalog hooks)
        {
            _configuration = configuration;
            _hooks = hooks;
        }

        public void Initialise()
        {
            var parser = new HooksParser(_hooks);

            foreach (var assembly in _configuration.Assemblies)
                parser.FindHooks(Assembly.LoadFrom(assembly));
        }
    }
}