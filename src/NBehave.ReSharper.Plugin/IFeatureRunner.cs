using System.Collections.Generic;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin
{
    public interface IFeatureRunner
    {
        FeatureResults Run(IEnumerable<string> featureFiles);
        FeatureResults DryRun(IEnumerable<string> featureFiles);
    }
}