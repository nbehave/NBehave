using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class BackgroundBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Feature _feature;
        private string _file;

        public BackgroundBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<FeatureBuilt>(built => _feature = built.Content, true);
            _hub.Subscribe<ParsingFileStart>(file => _file = file.Content, true);
            _hub.Subscribe<ParsedBackground>(
                message =>
                    {
                        var scenario = new Scenario(message.Content, _file);
                        _feature.AddBackground(scenario);

                        _hub.Publish(new BackgroundBuilt(this, scenario));
                    }, parsedScenario => _feature != null);
        }

        public override void Cleanup()
        {
            _feature = null;
            _file = string.Empty;
        }
    }
}