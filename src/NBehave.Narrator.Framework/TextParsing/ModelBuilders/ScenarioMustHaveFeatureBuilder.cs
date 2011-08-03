using System;
using System.Runtime.Serialization;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    internal class ScenarioMustHaveFeatureBuilder : AbstracModelBuilder
    {
        private Feature _feature;
        private readonly ITinyMessengerHub _hub;

        public ScenarioMustHaveFeatureBuilder(ITinyMessengerHub hub)
            : base(hub)
        {
            _hub = hub;
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

    [Serializable]
    public class ScenarioMustHaveFeatureException : Exception
    {
        private readonly string _file;

        public ScenarioMustHaveFeatureException(string file)
        {
            _file = file;
        }

        protected ScenarioMustHaveFeatureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _file = info.GetString("file");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("file", _file);
        }

        public override string Message
        {
            get { return string.Format("A scenario has been parsed that does not have a feature defined. Please add a feature to this scenario in the file: {0}.", _file); }
        }
    }
}