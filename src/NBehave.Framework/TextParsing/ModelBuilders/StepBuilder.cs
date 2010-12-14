namespace NBehave.Narrator.Framework.Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using NBehave.Narrator.Framework.Tiny;

    class StepBuilder : IModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Scenario _scenario;
        private readonly Stack<ParsedStep> _lastStep = new Stack<ParsedStep>();

        public StepBuilder(ITinyMessengerHub hub)
        {
            _hub = hub;

            _hub.Subscribe<ScenarioBuilt>(built => _scenario = built.Content);
            _hub.Subscribe<ParsedStep>(message => _lastStep.Push(message));

            _hub.Subscribe<ITinyMessage>(
                anyMessage =>
                {
                    if (!(anyMessage is ParsedTable))
                    {
                        _scenario.AddStep(_lastStep.Pop().Content);
                    }
                },
                _lastStep.Any());
        }
    }
}