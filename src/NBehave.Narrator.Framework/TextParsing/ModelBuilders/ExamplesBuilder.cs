using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    internal class ExamplesBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Scenario _scenario;
        private bool _listenToParsedTable;

        public ExamplesBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<ScenarioBuilt>(built => _scenario = built.Content, true);
            _hub.Subscribe<EnteringExamples>(_ => _listenToParsedTable = true, true);
            _hub.Subscribe<ParsedTable>(ExtractExamplesFromTable, _ => _listenToParsedTable);
        }

        private void ExtractExamplesFromTable(ParsedTable table)
        {
            if (!_listenToParsedTable)
                return;

            var columns = table.Content.First().Select(token => new ExampleColumn(token.Content)).ToList();
            var exampleColumns = new ExampleColumns(columns);

            foreach (var list in table.Content.Skip(1))
            {
                var example = list.Select(token => token.Content);
                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                    row.Add(exampleColumns[i].Name.ToLower(), example.ElementAt(i));

                _scenario.AddExamples(new List<Example> { new Example(exampleColumns, row) });
            }
            _listenToParsedTable = false;
        }

        public override void Cleanup()
        {
            _listenToParsedTable = false;
            _scenario = null;
        }
    }
}