using System.Collections.Generic;
using System.Linq;
using NBehave.Domain;
using NBehave.Internal.Gherkin;

namespace NBehave.TextParsing.ModelBuilders
{
    public class ExamplesBuilder
    {
        private Scenario scenario;
        private bool listenToParsedTable;

        public ExamplesBuilder(IGherkinParserEvents gherkinEvents)
        {
            gherkinEvents.FeatureEvent += (s, e) => Cleanup();
            gherkinEvents.EofEvent += (s, e) => Cleanup();
            gherkinEvents.ScenarioEvent += (sender, evt) => { scenario = evt.EventInfo; };
            gherkinEvents.ExamplesEvent += (sender, evt) => { listenToParsedTable = true; };
            gherkinEvents.TableEvent += (sender, evt) => ExtractExamplesFromTable(evt.EventInfo);
        }

        private void ExtractExamplesFromTable(IList<IList<Token>> table)
        {
            if (!listenToParsedTable)
                return;

            var columns = table.First().Select(token => new ExampleColumn(token.Content)).ToList();
            var exampleColumns = new ExampleColumns(columns);

            foreach (var list in table.Skip(1))
            {
                var example = list.Select(token => token.Content);
                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                    row.Add(exampleColumns[i].Name.ToLower(), example.ElementAt(i));

                scenario.AddExamples(new List<Example> { new Example(exampleColumns, row) });
            }
            listenToParsedTable = false;
        }

        private void Cleanup()
        {
            listenToParsedTable = false;
            scenario = null;
        }
    }
}