namespace NBehave.Narrator.Framework
{
    using System.Linq;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public static class NBehaveInitialiser
    {
        public static void Initialise(TinyIoCContainer container, NBehaveConfiguration configuration)
        {
            container.Register<ActionCatalog>().AsSingleton();
            container.Register(configuration);
            container.Register<IStringStepRunner, StringStepRunner>().AsMultiInstance();
            container.Register<ITinyMessengerHub, TinyMessengerHub>().AsSingleton();

            (from type in typeof(NBehaveInitialiser).Assembly.GetTypes()
             where type.GetInterfaces().Contains(typeof(IMessageProcessor))
             select container.Register(type).AsSingleton()).ToList();
            
            configuration.EventListener.Initialise(container.Resolve<ITinyMessengerHub>());

            Compose<IMessageProcessor>(container);
        }

        private static void Compose<T>(TinyIoCContainer container)
        {
            (from type in typeof(NBehaveInitialiser).Assembly.GetTypes()
             where type.GetInterfaces().Contains(typeof(T))
             select container.Resolve(type)).ToList().Cast<T>();
        }
    }
}