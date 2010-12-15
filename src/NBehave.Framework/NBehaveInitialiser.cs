namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    using TinyIoC;

    public static class NBehaveInitialiser
    {
        public static void Initialise(TinyIoCContainer container, NBehaveConfiguration configuration)
        {
            container.Register<ActionCatalog>().AsSingleton();
            container.Register(configuration);
            container.Register<IStringStepRunner, StringStepRunner>().AsMultiInstance();
            container.Register<ITinyMessengerHub, TinyMessengerHub>().AsSingleton();

            container.RegisterMany<IMessageProcessor>().AsSingleton();
            container.RegisterMany<IModelBuilder>().AsSingleton();

            configuration.EventListener.Initialise(container.Resolve<ITinyMessengerHub>());

            container.Resolve<IEnumerable<IMessageProcessor>>();
            container.Resolve<IEnumerable<IModelBuilder>>();
        }
    }
}