namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    internal class FeatureBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private string _file;

        public FeatureBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;
            _hub.Subscribe<ParsingFileStart>(file => _file = file.Content, true);
            _hub.Subscribe<ParsedFeature>(message =>
                {
                    var feature = new Feature(message.Content, _file);
                    _hub.Publish(new FeatureBuilt(this, feature));
                }, true);
        }
    }
}