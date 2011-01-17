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
        private readonly Queue<ParsedStep> _lastStep = new Queue<ParsedStep>();

        public StepBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<ScenarioBuilt>(built => _scenario = built.Content);
            _hub.Subscribe<ParsedStep>(message => _lastStep.Enqueue(message));

            _hub.Subscribe<ITinyMessage>(
                anyMessage =>
                {
                    if (!(anyMessage is ParsedTable))
                    {
                        _scenario.AddStep(_lastStep.Dequeue().Content);
                    }
                },
                tinyMessage => _lastStep.Any());
        }

        public override void Cleanup()
        {
            _scenario = null;
            _lastStep.Clear();
        }
    }
}