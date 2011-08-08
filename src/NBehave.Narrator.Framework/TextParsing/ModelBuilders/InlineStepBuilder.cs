using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    class InlineStepBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Scenario _scenario;
        private readonly Queue<ParsedStep> _lastStep = new Queue<ParsedStep>();

        public InlineStepBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;
            _hub.Subscribe<ScenarioBuilt>(built => _scenario = built.Content);
            _hub.Subscribe<ParsedStep>(message => _lastStep.Enqueue(message));

            _hub.Subscribe<ITinyMessage>(
                anyMessage =>
                {
                    if (anyMessage is ParsedTable && anyMessage != _lastStep.Peek())
                    {
                        ExtractInlineTableStepsFromTable(anyMessage);
                    }
                    else
                    {
                        _lastStep.Dequeue();
                    }
                },
                tinyMessage => _lastStep.Any());
        }

        private void ExtractInlineTableStepsFromTable(ITinyMessage anyMessage)
        {
            var stringTableStep = new StringTableStep(_lastStep.Dequeue().Content, _scenario.Source);
            _scenario.AddStep(stringTableStep);

            IList<IList<Token>> content = ((ParsedTable)anyMessage).Content;

            var columns = content.First().Select(token => new ExampleColumn(token.Content));
            var exampleColumns = new ExampleColumns(columns);

            foreach (var list in content.Skip(1))
            {
                var example = list.Select(token => token.Content);

                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                {
                    row.Add(exampleColumns[i].Name, example.ElementAt(i));
                }

                stringTableStep.AddTableStep(new Row(exampleColumns, row));
            }
        }

        public override void Cleanup()
        {
            _scenario = null;
            _lastStep.Clear();
        }
    }
}