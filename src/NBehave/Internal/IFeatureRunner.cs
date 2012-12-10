using NBehave.Domain;

namespace NBehave.Internal
{
    public interface IFeatureRunner
    {
        FeatureResult Run(Feature feature);
    }
}