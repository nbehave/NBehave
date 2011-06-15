using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{

    internal class ExamplesBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Scenario _scenario;
        private bool _midExample;

        public ExamplesBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<ScenarioBuilt>(built => _scenario = built.Content);
            _hub.Subscribe<EnteringExamples>(examples => _midExample = true);
            _hub.Subscribe<ParsedTable>(ExtractExamplesFromTable, parsedTable => _midExample);
        }

        private void ExtractExamplesFromTable(ParsedTable table)
        {
            var columns = table.Content.First().Select(token => new ExampleColumn(token.Content));
            var exampleColumns = new ExampleColumns(columns);

            foreach (var list in table.Content.Skip(1))
            {
                var example = list.Select(token => token.Content);
                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                    row.Add(exampleColumns[i].Name.ToLower(), example.ElementAt(i));

                _scenario.AddExamples(new List<Example> { new Example(exampleColumns, row) });
            }

            _midExample = false;
        }

        public override void Cleanup()
        {
            _scenario = null;
            _midExample = false;
        }
    }
}