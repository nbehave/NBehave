namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    class ScenarioBuilder : IModelBuilder
    {
        private readonly ITinyMessengerHub _hub;
        private Feature _feature;
        private string _file;

        public ScenarioBuilder(ITinyMessengerHub hub)
        {
            _hub = hub;

            _hub.Subscribe<FeatureBuilt>(built => _feature = built.Content);
            _hub.Subscribe<ParsingFile>(file => this._file = file.Content);
            _hub.Subscribe<ParsedScenario>(
                message =>
                    {
                        var scenario = new Scenario(message.Content)
                            {
                                Source = this._file
                            };

                        _feature.AddScenario(scenario);

                        _hub.Publish(new ScenarioBuilt(this, scenario));
                    });
        }
    }
}