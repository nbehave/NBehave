namespace NBehave.Narrator.Framework
{
    using System.Linq;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public static class NBehaveInitialiser
    {
        public static void Initialise(TinyIoCContainer container, NBehaveConfiguration configuration)
        {
            container.AutoRegister(typeof(NBehaveInitialiser).Assembly);
            container.Register<ActionCatalog>().AsSingleton();
            container.Register(configuration);
            configuration.EventListener.Initialise(container.Resolve<ITinyMessengerHub>());

            Compose<IMessageProcessor>(container);
        }

        private static void Compose<T>(TinyIoCContainer container)
        {
            (from type in typeof(NBehaveInitialiser).Assembly.GetTypes()
             where type.GetInterfaces().Contains(typeof(T))
             select container.Resolve(type)).ToList().Cast<T>();
            return;
        }
    }
}