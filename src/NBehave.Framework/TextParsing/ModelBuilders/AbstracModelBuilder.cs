namespace NBehave.Narrator.Framework.Processors
{
    using System;

    using NBehave.Narrator.Framework.Tiny;

    internal abstract class AbstracModelBuilder : IModelBuilder
    {
        protected readonly ITinyMessengerHub _hub;

        public AbstracModelBuilder(ITinyMessengerHub hub)
        {
            _hub = hub;

            _hub.Subscribe<ModelBuilderInitialise>(start => this.Initialise());
            _hub.Subscribe<ModelBuilderCleanup>(start => this.Cleanup());
        }

        public virtual void Initialise(){}
        public virtual void Cleanup(){}
    }
}