namespace NBehave.Narrator.Framework.Processors
{
    using System;

    using NBehave.Narrator.Framework.Tiny;

    internal class ScenarioMustHaveFeatureBuilder : AbstracModelBuilder
    {
        private Feature _feature;
        
        public ScenarioMustHaveFeatureBuilder(ITinyMessengerHub hub)
            : base(hub)
        {

            _hub.Subscribe<FeatureBuilt>(built => _feature = built.Content);
            _hub.Subscribe<ParsedScenario>(scenario =>
                {
                    if (_feature == null)
                    {
                        throw new ScenarioMustHaveFeatureException(scenario.Content);
                    }
                });
        }

        public override void Cleanup()
        {
            _feature = null;
        }
    }

    internal class ScenarioMustHaveFeatureException : Exception
    {
        private string _file;

        public ScenarioMustHaveFeatureException(string file)
        {
            _file = file;
        }

        public override string Message
        {
            get
            {
                return string.Format("A scenario has been parsed that does not have a feature defined. Please add a feature to this scenario in the file: {0}.", _file);
            }
        }
    }
}