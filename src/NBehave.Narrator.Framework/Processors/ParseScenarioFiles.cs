using System.Collections.Generic;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Contracts
{
    public class ParseScenarioFiles : IMessageProcessor
    {
        private readonly ITinyMessengerHub _hub;
        private readonly List<Feature> _features;
        private NBehaveConfiguration _configuration;

        public ParseScenarioFiles(ITinyMessengerHub hub, NBehaveConfiguration configuration)
        {
            _hub = hub;
            _features = new List<Feature>();
            _configuration = configuration;

            _hub.Subscribe<ScenarioFilesLoaded>(loaded => OnScenarioFilesLoaded(loaded.Content), true);
            _hub.Subscribe<FeatureBuilt>(built => _features.Add(built.Content), true);
        }

        private void OnScenarioFilesLoaded(IEnumerable<string> content)
        {
            LoadFiles(content);
            _hub.Publish(new FeaturesLoaded(this, _features));
        }

        private void LoadFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var scenarioTextParser = new GherkinScenarioParser(_hub, _configuration);
                scenarioTextParser.Parse(file);
            }
        }
    }
}