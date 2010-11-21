namespace NBehave.Narrator.Framework.Contracts
{
    using System.Reflection;

    public class LoadActionSteps : IStartupTask
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
            var parser = new ActionStepParser(this._configuration.Filter, this._actionCatalog);

            foreach (var assembly in _configuration.Assemblies)
            {
                parser.FindActionSteps(Assembly.LoadFrom(assembly));
            }
        }
    }
}