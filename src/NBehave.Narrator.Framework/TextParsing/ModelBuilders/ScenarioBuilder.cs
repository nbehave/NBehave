using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class ScenarioBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Feature _feature;
        private string _file;

        public ScenarioBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<FeatureBuilt>(built => _feature = built.Content, true);
            _hub.Subscribe<ParsingFileStart>(file => _file = file.Content, true);
            _hub.Subscribe<ParsedScenario>(
                message =>
                    {
                        var scenario = new Scenario(message.Content, _file);
                        _feature.AddScenario(scenario);

                        _hub.Publish(new ScenarioBuilt(this, scenario));
                    }, parsedScenario => _feature != null);
        }

        public override void Cleanup()
        {
            _feature = null;
            _file = string.Empty;
        }
    }
}