using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public class FeatureRunner : IFeatureRunner, Narrator.Framework.Internal.IFeatureRunner
    {
        private readonly NBehaveConfiguration configuration;
        private readonly IStringStepRunner stringStepRunner;
        private readonly IRunContext context;

        public FeatureRunner()
        {
            configuration = TinyIoCContainer.Current.Resolve<NBehaveConfiguration>();
            stringStepRunner = TinyIoCContainer.Current.Resolve<IStringStepRunner>();
            context = TinyIoCContainer.Current.Resolve<IRunContext>();
        }

        FeatureResults IFeatureRunner.Run(IEnumerable<string> featureFiles)
        {
            configuration.IsDryRun = false;
            return Run(featureFiles);
        }

        public FeatureResults DryRun(IEnumerable<string> featureFiles)
        {
            configuration.IsDryRun = true;
            return Run(featureFiles);
        }

        private FeatureResults Run(IEnumerable<string> featureFiles)
        {
            configuration.SetScenarioFiles(featureFiles);
            var t = new TextRunner(configuration);
            return t.Run();
        }

        FeatureResult Narrator.Framework.Internal.IFeatureRunner.Run(Feature feature)
        {
            if (configuration.IsDryRun)
                return new FeatureResult();
            return DoRun(feature);
        }

        private FeatureResult DoRun(Feature feature)
        {
            var runner = new Narrator.Framework.Internal.FeatureRunner(stringStepRunner, context);
            return runner.Run(feature);
        }
    }
}