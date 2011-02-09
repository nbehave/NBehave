namespace NBehave.Narrator.Framework.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NBehave.Narrator.Framework.Tiny;

    class StepBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Scenario _scenario;

        public StepBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            Zip(_hub, (messageOne, messageTwo) =>
                {
                    //HACK: "_scenario == null" implies that the parsing is going wrong. It would be better to add some filter predicate to our message bus subscription.
                    if(messageOne is ParsedStep && !(messageTwo is ParsedTable) && _scenario != null)
                    {
                        _scenario.AddStep((messageOne as ParsedStep).Content);
                    }

                    if (messageOne is ScenarioBuilt) 
                        _scenario = (messageOne as ScenarioBuilt).Content;
                    if (messageTwo is ScenarioBuilt)
                        _scenario = (messageTwo as ScenarioBuilt).Content;
                });
        }

        private void Zip(ITinyMessengerHub hub, Action<ITinyMessage, ITinyMessage> zipped)
        {
            var queue = new Queue<ITinyMessage>();
            hub.Subscribe<ITinyMessage>(message =>
                {
                    queue.Enqueue(message);

                    if (queue.Count == 2)
                    {
                        zipped(queue.Dequeue(), queue.Peek());
                    }
                });
        }

        public override void Cleanup()
        {
            _scenario = null;
        }
    }
}