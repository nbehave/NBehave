using System.Collections.Generic;
using System.Linq;
using NBehave.Domain;
using NBehave.Internal.Gherkin;

namespace NBehave.TextParsing.ModelBuilders
{
    public class StepBuilder
    {
        private Scenario scenario;
        private StringStep previousStep;
        public StepBuilder(IGherkinParserEvents gherkinEvents)
        {
            gherkinEvents.ScenarioEvent += (s, e) =>
                                               {
                                                   HandlePreviousEvent();
                                                   scenario = e.EventInfo;
                                               };
            gherkinEvents.StepEvent += (s, e) =>
                                           {
                                               HandlePreviousEvent();
                                               previousStep = e.EventInfo;
                                           };
            gherkinEvents.DocStringEvent += (s, e) => HandleDocString(e.EventInfo);
            gherkinEvents.FeatureEvent += (s, e) => HandlePreviousEventAndCleanUp();
            gherkinEvents.ExamplesEvent += (s, e) => HandlePreviousEvent();
            gherkinEvents.BackgroundEvent += (s, e) =>
                                                 {
                                                     HandlePreviousEvent();
                                                     scenario = e.EventInfo;
                                                 };
            gherkinEvents.TableEvent += (s, e) =>
                                            {
                                                HandleTableEvent(e.EventInfo);
                                                previousStep = null;
                                            };
            gherkinEvents.TagEvent += (s, e) => HandlePreviousEvent();
            gherkinEvents.EofEvent += (s, e) => HandlePreviousEventAndCleanUp();
        }

        private void HandleDocString(string docString)
        {
            if (previousStep == null || scenario == null)
                return;
            HandlePreviousEvent();
            scenario.Steps.Last().AddDocString(docString);
        }

        private void HandlePreviousEventAndCleanUp()
        {
            HandlePreviousEvent();
            Cleanup();
        }

        private void HandleTableEvent(IList<IList<Token>> content)
        {
            if (previousStep == null)
                return;
            var stringTableStep = new StringTableStep(previousStep.Step, scenario.Source, previousStep.SourceLine);
            scenario.AddStep(stringTableStep);

            var columns = content.First().Select(token => new ExampleColumn(token.Content));
            var exampleColumns = new ExampleColumns(columns);

            foreach (var list in content.Skip(1))
            {
                var example = list.Select(token => token.Content).ToList();

                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                    row.Add(exampleColumns[i].Name, example.ElementAt(i));

                stringTableStep.AddTableStep(new Example(exampleColumns, row));
            }
        }

        private void HandlePreviousEvent()
        {
            if (previousStep == null || scenario == null)
                return;
            scenario.AddStep(previousStep);
            previousStep = null;
        }

        private void Cleanup()
        {
            scenario = null;
            previousStep = null;
        }
    }
}