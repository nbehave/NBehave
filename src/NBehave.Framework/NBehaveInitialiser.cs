namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;
    using System.Linq;

    using NBehave.Narrator.Framework.Contracts;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public static class NBehaveInitialiser
    {
        public static void Initialise(TinyIoCContainer container)
        {
            IEnumerable<IStartupTask> startupTasks = Compose<IStartupTask>(container);
            IEnumerable<IMessageProcessor> processors = Compose<IMessageProcessor>(container);

            startupTasks.Each(startupTask => startupTask.Initialise());
            processors.Each(startupTask => startupTask.Start());
        }

        private static IEnumerable<T> Compose<T>(TinyIoCContainer container)
        {
            return (from type in typeof(NBehaveInitialiser).Assembly.GetTypes()
                    where type.GetInterfaces().Contains(typeof(T))
                    select container.Resolve(type)).ToList().Cast<T>();
        }
    }
}