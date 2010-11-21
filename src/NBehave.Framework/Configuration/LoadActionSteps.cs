namespace NBehave.Narrator.Framework.Contracts
{
    using System.Reflection;

    class LoadActionSteps : IStartupTask
    {
        private NBehaveConfiguration _configuration;

        private readonly ActionCatalog _actionCatalog;

        public LoadActionSteps(NBehaveConfiguration configuration, ActionCatalog actionCatalog)
        {
            this._configuration = configuration;
            _actionCatalog = actionCatalog;
        }

        public void Initialise()
        {
            foreach (var assembly in _configuration.Assemblies)
            {
                LoadAssembly(assembly);
            }
        }

        private void LoadAssembly(string assemblyPath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyPath));
        }

        private void LoadAssembly(Assembly assembly)
        {
            ParseAssembly(assembly);
        }

        private void ParseAssembly(Assembly assembly)
        {
            var parser = new ActionStepParser(_configuration.Filter, _actionCatalog);
            parser.FindActionSteps(assembly);
        }
    }
}