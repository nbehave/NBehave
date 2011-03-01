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
        private Scenario _background;

        public StepBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            Zip(_hub, (messageOne, messageTwo) =>
                {
                    if(messageOne is ParsedStep && !(messageTwo is ParsedTable))
                    {
                        //"_scenario == null" implies that we are parsing the background steps
                        if (_scenario == null)
                        {
                            _background.AddStep((messageOne as ParsedStep).Content);
                        }
                        else
                        {
                            _scenario.AddStep((messageOne as ParsedStep).Content);
                        }
                    }

                    if (messageOne is ScenarioBuilt) 
                        _scenario = (messageOne as ScenarioBuilt).Content;
                    if (messageTwo is ScenarioBuilt)
                        _scenario = (messageTwo as ScenarioBuilt).Content;
                    if (messageOne is BackgroundBuilt)
                        _background = (messageOne as BackgroundBuilt).Content;
                    if (messageTwo is BackgroundBuilt)
                        _background = (messageTwo as BackgroundBuilt).Content;
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