using System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public static class NBehaveInitialiser
    {
        public static void Initialise(NBehaveConfiguration configuration)
        {
            TinyIoCContainer container = TinyIoCContainer.Current;
            container.Register<IFeatureRunner, FeatureRunner>();
            CommonInitializer.Initialise(container, configuration);
            if (configuration.CreateAppDomain)
                RegisterEventListener(configuration.EventListener, container.Resolve<ITinyMessengerHub>());
        }

        /// <summary>
        ///   Connects an event listener to our message bus
        /// </summary>
        /// <param name = "listener">The event listener, which will be marshalled from another AppDomain</param>
        /// <param name = "hub">The message bus</param>
        /// <remarks>
        ///   We cannot pass the message bus instance to the event listener, since the listener may be in a remote AppDomain
        /// </remarks>
        private static void RegisterEventListener(IEventListener listener, ITinyMessengerHub hub)
        {
            hub.Subscribe<FeatureStartedEvent>(_ => listener.FeatureStarted(_.Content), true);
            hub.Subscribe<FeatureResultEvent>(_ => listener.FeatureFinished(_.Content), true);
            hub.Subscribe<FeatureNarrativeEvent>(_ => listener.FeatureNarrative(_.Content), true);
            hub.Subscribe<ScenarioStartedEvent>(_ => listener.ScenarioStarted(_.Content.Title), true);
            hub.Subscribe<RunStartedEvent>(_ => listener.RunStarted(), true);
            hub.Subscribe<RunFinishedEvent>(_ => listener.RunFinished(), true);
            hub.Subscribe<ScenarioResultEvent>(_ => listener.ScenarioFinished(_.Content), true);
        }
    }

    public static class CommonInitializer
    {
        public static void Initialise(TinyIoCContainer container, NBehaveConfiguration configuration)
        {
            InitializeContext(container);

            container.Register<ActionCatalog>().AsSingleton();
            container.Register(configuration);
            container.Register<IStringStepRunner, StringStepRunner>().AsMultiInstance();
            container.Register<ITinyMessengerHub, TinyMessengerHub>().AsSingleton();

            container.RegisterMany<IMessageProcessor>().AsSingleton();
            container.RegisterMany<IModelBuilder>().AsSingleton();

            container.Resolve<IEnumerable<IMessageProcessor>>();
            container.Resolve<IEnumerable<IModelBuilder>>();
        }

        private static void InitializeContext(TinyIoCContainer container)
        {
            container.Register<FeatureContext>().AsSingleton();
            container.Register<ScenarioContext>().AsSingleton();
            container.Register<StepContext>().AsSingleton();
        }
    }
}