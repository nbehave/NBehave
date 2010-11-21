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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NBehave.Narrator.Framework.Contracts;
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public class TextRunner
    {
        private readonly NBehaveConfiguration _configuration;

        private EventHandler<EventArgs<Feature>> _featureCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioWithSteps>> _scenarioCreatedEventHandler;
        private EventHandler<EventArgs<ScenarioResult>> _scenarioResultAddedEventHandler;
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

            IEnumerable<IStartupTask> startupTasks =
                (from type in this.GetType().Assembly.GetTypes()
                where type.GetInterfaces().Contains(typeof(IStartupTask))
                select container.Resolve(type) as IStartupTask).ToList();

            IEnumerable<IMessageProcessor> processors =
                (from type in this.GetType().Assembly.GetTypes()
                where type.GetInterfaces().Contains(typeof(IMessageProcessor))
                select container.Resolve(type) as IMessageProcessor).ToList();

            startupTasks.Each(startupTask => startupTask.Initialise());
            processors.Each(startupTask => startupTask.Start());

            FeatureResults results = null;
            _hub.Subscribe<FeatureResults>(featureResults => results = featureResults);
            try
            {
                StartWatching();    
                InitializeRun();
            }
            finally
            {
                StopWatching();
            }

            return results;
        }

        private void StartWatching()
        {
            StartWatchingFeatureCreated(_configuration.EventListener);
            StartWatchingFeatureResults(_configuration.EventListener);
            StartWatchingScenarioCreated(_configuration.EventListener);
        }

        private void StartWatchingFeatureResults(IEventListener listener)
        {
            _scenarioResultAddedEventHandler = (sender, e) => listener.ScenarioResult(e.EventData);
            ScenarioStepRunner.ScenarioResultCreated += _scenarioResultAddedEventHandler;
        }

        private void StartWatchingScenarioCreated(IEventListener listener)
        {
            _scenarioCreatedEventHandler = (sender, e) => listener.ScenarioCreated(e.EventData.Title);
            ScenarioWithSteps.ScenarioCreated += _scenarioCreatedEventHandler;
        }

        private void StartWatchingFeatureCreated(IEventListener listener)
        {
            _featureCreatedEventHandler = (sender, e) =>
            {
                e.EventData.IsDryRun = _configuration.IsDryRun;
                listener.FeatureCreated(e.EventData.Title);
                listener.FeatureNarrative(e.EventData.Narrative);
            };
            Feature.FeatureCreated += _featureCreatedEventHandler;
        }

        private void StopWatching()
        {
            Feature.FeatureCreated -= _featureCreatedEventHandler;
            ScenarioWithSteps.ScenarioCreated -= _scenarioCreatedEventHandler;
            ScenarioStepRunner.ScenarioResultCreated -= _scenarioResultAddedEventHandler;
            _configuration.EventListener.RunFinished();
        }

        private void InitializeRun()
        {
            _configuration.EventListener.RunStarted();
            _hub.Publish(new RunStarted(this));
        }
    }
}
