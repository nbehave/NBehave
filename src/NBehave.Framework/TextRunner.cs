// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRunner.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the TextRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class TextRunner
    {
        private readonly NBehaveConfiguration _configuration;
        private ITinyMessengerHub _hub;

        public TextRunner(NBehaveConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FeatureResults Run()
        {
            var container = TinyIoCContainer.Current;
            container.AutoRegister(this.GetType().Assembly);
            container.Register<ActionCatalog>().AsSingleton();
            container.Register(_configuration);
            container.Register<ActionStepFileLoader>().AsSingleton();
            _hub = container.Resolve<ITinyMessengerHub>();

            _configuration.EventListener.Initialise(_hub);

            NBehaveInitialiser.Initialise(container);

            FeatureResults results = null;
            _hub.Subscribe<FeatureResults>(featureResults => results = featureResults);
            
            try
            {
                InitializeRun();
            }
            finally
            {
                StopWatching();
            }

            return results;
        }

        private void StopWatching()
        {
            _hub.Publish(new RunFinished(this));
        }

        private void InitializeRun()
        {
            _hub.Publish(new RunStarted(this));
        }
    }
}
