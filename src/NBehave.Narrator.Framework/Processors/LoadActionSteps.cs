using System.Reflection;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Contracts
{
    public class LoadActionSteps : IMessageProcessor
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ActionCatalog _actionCatalog;
        private readonly ITinyMessengerHub _hub;

        public LoadActionSteps(NBehaveConfiguration configuration, ActionCatalog actionCatalog, ITinyMessengerHub hub)
        {
            _configuration = configuration;
            _actionCatalog = actionCatalog;
            _hub = hub;

            _hub.Subscribe<RunStartedEvent>(started => Initialise(), true);
        }

        private void Initialise()
        {
            var parser = new ActionStepParser(_configuration.Filter, _actionCatalog);

            foreach (var assembly in _configuration.Assemblies)
            {
                parser.FindActionSteps(Assembly.LoadFrom(assembly));
            }

            _hub.Publish(new ActionStepsLoaded(this));
        }
    }
}