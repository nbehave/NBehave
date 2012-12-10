using System;
using System.Linq;
using NBehave.Domain;


namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public class FeatureGherkinText : IGherkinText
    {
        private readonly Feature feature;

        public FeatureGherkinText(Feature feature)
        {
            this.feature = feature;
        }

        public string AsString
        {
            get
            {
                return feature + Environment.NewLine +
                       String.Join(Environment.NewLine, feature.Scenarios.Select(_=>_.ToString()));
            }
        }
        public string Title { get { return feature.Title; } }
        public int SourceLine { get { return feature.SourceLine; } }
    }
}