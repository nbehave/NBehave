namespace NBehave.Narrator.Framework.Contracts
{
    using System.Reflection;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public class LoadActionSteps : IMessageProcessor
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ActionCatalog _actionCatalog;
        private readonly ITinyMessengerHub _hub;

        public LoadActionSteps(NBehaveConfiguration configuration, ActionCatalog actionCatalog, ITinyMessengerHub hub)
        {
            this._configuration = configuration;
            _actionCatalog = actionCatalog;
            _hub = hub;

            this._hub.Subscribe<RunStartedEvent>(started => this.Initialise());
        }

        public void Initialise()
        {
            var parser = new ActionStepParser(this._configuration.Filter, this._actionCatalog);

            foreach (var assembly in _configuration.Assemblies)
            {
                parser.FindActionSteps(Assembly.LoadFrom(assembly));
            }

            _hub.Publish(new ActionStepsLoaded(this));
        }
    }
}