namespace NBehave.Narrator.Framework.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NBehave.Narrator.Framework.Tiny;

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
            _hub.Subscribe<EnteringExamples>(examples => this._midExample = true);
            _hub.Subscribe<ParsedTable>(this.ExtractExamplesFromTable, parsedTable => _midExample);
        }

        private void ExtractExamplesFromTable(ParsedTable table)
        {
            foreach (var list in table.Content)
            {
                var exampleColumns = new ExampleColumns(list.Select(token => token.Content.ToLower()));
                            
                var example = list.Select(token => token.Content);

                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                {
                    row.Add(exampleColumns[i], example.ElementAt(i));
                }

                this._scenario.AddExamples(new List<Example> { new Example(exampleColumns, row) });
            }

            _midExample = false;
        }

        public override void Ending()
        {
            _scenario = null;
            _midExample = false;
        }
    }
}