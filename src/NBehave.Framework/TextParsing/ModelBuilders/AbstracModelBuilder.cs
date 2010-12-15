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

            _hub.Subscribe<ParsingFileStart>(start => this.Starting());
            _hub.Subscribe<ParsingFileEnd>(start => this.Ending());
        }

        public virtual void Starting(){}
        public virtual void Ending(){}
    }
}