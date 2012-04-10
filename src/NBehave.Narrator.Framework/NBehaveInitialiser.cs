using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Hooks;
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
            //if (configuration.CreateAppDomain)
            RegisterEventListener(configuration.EventListener, container.Resolve<IRunContextEvents>());
        }

        /// <summary>
        ///   Connects an event listener to our message bus
        /// </summary>
        /// <param name = "listener">The event listener, which will be marshalled from another AppDomain</param>
        /// <param name = "context">Run context event raiser</param>
        /// <remarks>
        ///   We cannot pass the message bus instance to the event listener, since the listener may be in a remote AppDomain
        /// </remarks>
        private static void RegisterEventListener(IEventListener listener, IRunContextEvents context)
        {
            context.OnRunStarted += (s, e) => listener.RunStarted();
            context.OnRunFinished += (s, e) => listener.RunFinished();
            context.OnFeatureStarted += (s, e) => listener.FeatureStarted(e.EventInfo);
            context.OnFeatureFinished += (s, e) => listener.FeatureFinished(e.EventInfo);
            context.OnScenarioStarted += (s, e) => listener.ScenarioStarted(e.EventInfo.Title);
            context.OnScenarioFinished += (s, e) => listener.ScenarioFinished(e.EventInfo);
        }
    }

    public static class CommonInitializer
    {
        public static void Initialise(TinyIoCContainer container, NBehaveConfiguration configuration)
        {
            container.Register<ActionCatalog>().AsSingleton();
            container.Register<HooksCatalog>().AsSingleton();
            container.Register<HooksHandler>().AsSingleton();
            container.Register(configuration);
            container.Register<IStringStepRunner, StringStepRunner>().AsMultiInstance();

            InitializeContext(container);
            InitializeHooks(configuration, container.Resolve<HooksCatalog>());
        }

        private static void InitializeHooks(NBehaveConfiguration configuration, HooksCatalog hooksCatalog)
        {
            new LoadHooks(configuration, hooksCatalog).Initialise();
        }

        private static void InitializeContext(TinyIoCContainer container)
        {
            container.Register<FeatureContext>().AsSingleton();
            container.Register<ScenarioContext>().AsSingleton();
            container.Register<StepContext>().AsSingleton();
            container.Register<IContextHandler, ContextHandler>().AsSingleton();
            var r = new RunContext(container.Resolve<IContextHandler>(), container.Resolve<HooksHandler>());
            container.Register<IRunContext>(r);
            container.Register<IRunContextEvents>(r);
        }
    }
}