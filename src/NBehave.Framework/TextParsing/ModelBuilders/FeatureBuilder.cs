namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureBuilder : IModelBuilder
    {
        private readonly ITinyMessengerHub _hub;

        public FeatureBuilder(ITinyMessengerHub hub)
        {
            _hub = hub;

            _hub.Subscribe<ParsedFeature>(message =>
                {
                    var feature = new Feature(message.Content);
                    _hub.Publish(new FeatureBuilt(this, feature));
                });
        }
    }
}