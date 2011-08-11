using System.Collections.Generic;

namespace NBehave.ReSharper.Plugin
{
    public interface IFeatureRunner
    {
        void Run(IEnumerable<string> featureFiles);
        void DryRun(IEnumerable<string> featureFiles);
    }
}