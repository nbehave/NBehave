namespace NBehave.Narrator.Framework.Contracts
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class ParseScenarioFiles : IStartupTask
    {
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;

        public ParseScenarioFiles(ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            this._stringStepRunner = stringStepRunner;
            this._hub = hub;

            this._hub.Subscribe<ScenarioFilesLoaded>(loaded => OnScenarioFilesLoaded(loaded.Content));
        }

        private void OnScenarioFilesLoaded(IEnumerable<string> content)
        {
            IEnumerable<Feature> features = this.LoadFiles(content);
            this._hub.Publish(new FeaturesLoaded(this, features));
        }

        private IEnumerable<Feature> LoadFiles(IEnumerable<string> files)
        {
            var stories = new List<Feature>();
            foreach (var file in files)
            {
                var scenarios = GetScenarios(file);
                stories.AddRange(scenarios);
            }

            return stories;
        }

        private IEnumerable<Feature> GetScenarios(string file)
        {
            IEnumerable<Feature> features;
            using (Stream stream = File.OpenRead(file))
            {
                var scenarioTextParser = new GherkinScenarioParser(this._stringStepRunner, this._hub);
                features = scenarioTextParser.Parse(stream);

                foreach (var scenario in features.SelectMany(feature => feature.Scenarios))
                {
                    scenario.Source = file;
                }
            }

            return features;
        }

        public void Initialise()
        {
        }
    }
}