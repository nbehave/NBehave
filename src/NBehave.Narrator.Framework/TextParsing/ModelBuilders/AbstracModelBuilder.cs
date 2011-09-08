using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public abstract class AbstracModelBuilder : IModelBuilder
    {
        private readonly ITinyMessengerHub _hub;

        protected AbstracModelBuilder(ITinyMessengerHub hub)
        {
            _hub = hub;

            _hub.Subscribe<ModelBuilderInitialise>(start => Initialise(), true);
            _hub.Subscribe<ModelBuilderCleanup>(start => Cleanup(), true);
        }

        public virtual void Initialise()
        {
        }

        public virtual void Cleanup()
        {
        }
    }
}