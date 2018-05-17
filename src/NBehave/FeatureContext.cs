using System.Collections.Generic;
using NBehave.Internal;

namespace NBehave
{
    public class FeatureContext : NBehaveContext
    {
        public string FeatureTitle { get { return Feature.Title; } }
        internal Feature Feature { get; set; }

        public FeatureContext(Feature feature)
        {
            Feature = feature;
        }

        public FeatureContext(Feature feature, IEnumerable<string> tags)
            :this(feature)
        {
            AddTags(tags);
        }

        public static FeatureContext Current
        {
            get { return TinyIoC.TinyIoCContainer.Current.Resolve<FeatureContext>(); }
        }

        public override string ToString()
        {
            return FeatureTitle;
        }
    }
}
