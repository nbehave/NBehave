namespace NBehave.Narrator.Framework.Processors
{
    using System;

    using NBehave.Narrator.Framework.Tiny;

    class BackgroundBuilder : AbstracModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Feature _feature;
        private string _file;

        public BackgroundBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;

            _hub.Subscribe<FeatureBuilt>(built => _feature = built.Content);
            _hub.Subscribe<ParsingFileStart>(file => this._file = file.Content);
            _hub.Subscribe<ParsedBackground>(
                message =>
                    {
                        var scenario = new Scenario(message.Content)
                            {
                                Source = this._file
                            };

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