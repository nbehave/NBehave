namespace NBehave.Narrator.Framework.Contracts
{
    using System.Collections.Generic;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public class ParseScenarioFiles : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;
        private readonly List<Feature> _features;

        public ParseScenarioFiles(ITinyMessengerHub hub)
        {
            this._hub = hub;
            this._features = new List<Feature>();

            this._hub.Subscribe<ScenarioFilesLoaded>(loaded => OnScenarioFilesLoaded(loaded.Content));
            this._hub.Subscribe<FeatureBuilt>(built => this._features.Add(built.Content));
        }

        private void OnScenarioFilesLoaded(IEnumerable<string> content)
        {
            this.LoadFiles(content);
            this._hub.Publish(new FeaturesLoaded(this, this._features));
        }

        private void LoadFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var scenarioTextParser = new GherkinScenarioParser(this._hub);
                scenarioTextParser.Parse(file);
            }
        }
    }
}