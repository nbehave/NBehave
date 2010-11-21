namespace NBehave.Narrator.Framework.Contracts
{
    using System;
    using System.Collections.Generic;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class LoadScenarioFiles : IStartupTask
    {
        private NBehaveConfiguration _configuration;

        private readonly ActionStepFileLoader _actionStepFileLoader;

        private readonly ITinyMessengerHub _hub;

        public LoadScenarioFiles(
            NBehaveConfiguration configuration, 
            ActionStepFileLoader actionStepFileLoader,
            ITinyMessengerHub hub)
        {
            this._configuration = configuration;
            _actionStepFileLoader = actionStepFileLoader;
            _hub = hub;
        }

        public void Initialise()
        {
            this.Load(_configuration.ScenarioFiles);
        }

        private void Load(IEnumerable<string> fileLocations)
        {
            var features = _actionStepFileLoader.Load(fileLocations);
            _hub.Publish(new FeaturesLoaded(this, features));
        }
    }
}