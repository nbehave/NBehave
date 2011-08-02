using System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public static class NBehaveInitialiser
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

            RegisterEventListener(configuration.EventListener, container.Resolve<ITinyMessengerHub>());

            container.Resolve<IEnumerable<IMessageProcessor>>();
            container.Resolve<IEnumerable<IModelBuilder>>();
        }

        private static void InitializeContext(TinyIoCContainer container)
        {
            container.Register<FeatureContext>().AsSingleton();
            container.Register<ScenarioContext>().AsSingleton();
            container.Register<StepContext>().AsSingleton();
        }

        /// <summary>
        ///   Connects an event listener to our message bus
        /// </summary>
        /// <param name = "listener">The event listener, which could be marshalled from another AppDomain</param>
        /// <param name = "hub">The message bus</param>
        /// <remarks>
        ///   We cannot pass the message bus instance to the event listener, since the listener may be in a remote AppDomain
        /// </remarks>
        private static void RegisterEventListener(IEventListener listener, ITinyMessengerHub hub)
        {
            hub.Subscribe<FeatureStartedEvent>(created => listener.FeatureCreated(created.Content));
            hub.Subscribe<FeatureNarrativeEvent>(narrative => listener.FeatureNarrative(narrative.Content));
            hub.Subscribe<ScenarioStartedEvent>(created => listener.ScenarioCreated(created.Content.Title));
            hub.Subscribe<RunStartedEvent>(started => listener.RunStarted());
            hub.Subscribe<RunFinishedEvent>(finished => listener.RunFinished());
            hub.Subscribe<ThemeStartedEvent>(themeStarted => listener.ThemeStarted(themeStarted.Content));
            hub.Subscribe<ThemeFinishedEvent>(themeFinished => listener.ThemeFinished());
            hub.Subscribe<ScenarioResultEvent>(message => listener.ScenarioResult(message.Content));
        }
    }
}