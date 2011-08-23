using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public class FeatureRunner : IFeatureRunner, Narrator.Framework.Processors.IFeatureRunner
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly IStringStepRunner _stringStepRunner;
        private readonly ITinyMessengerHub _hub;

        public FeatureRunner()
        {
            _configuration = TinyIoCContainer.Current.Resolve<NBehaveConfiguration>();
            _stringStepRunner = TinyIoCContainer.Current.Resolve<IStringStepRunner>();
            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
        }

        void IFeatureRunner.Run(IEnumerable<string> featureFiles)
        {
            _configuration.IsDryRun = false;
            Run(featureFiles);
        }

        public void DryRun(IEnumerable<string> featureFiles)
        {
            _configuration.IsDryRun = true;
            Run(featureFiles);
        }

        private void Run(IEnumerable<string> featureFiles)
        {
            _configuration.SetScenarioFiles(featureFiles);

            try
            {
                _hub.Publish(new RunStartedEvent(this));
            }
            finally
            {
                _hub.Publish(new RunFinishedEvent(this));
            }
        }

        void Narrator.Framework.Processors.IFeatureRunner.Run(Feature feature)
        {
            if (_configuration.IsDryRun)
                return ;
            DoRun(feature);
        }

        private void DoRun(Feature feature)
        {
            var runner = new Narrator.Framework.Processors.FeatureRunner(_hub, _stringStepRunner);
            runner.Run(feature);
        }
    }
}