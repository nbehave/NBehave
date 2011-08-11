using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public class FeatureRunner : IFeatureRunner
    {
        private readonly ITinyMessengerHub _hub;
        private readonly NBehaveConfiguration _config;

        public FeatureRunner()
        {
            _config = TinyIoCContainer.Current.Resolve<NBehaveConfiguration>();
            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
        }

        void IFeatureRunner.Run(IEnumerable<string> featureFiles)
        {
            _config.IsDryRun = false;
            Run(featureFiles);
        }

        public void DryRun(IEnumerable<string> featureFiles)
        {
            _config.IsDryRun = true;
            Run(featureFiles);
        }

        private void Run(IEnumerable<string> featureFiles)
        {
            _config.SetScenarioFiles(featureFiles);

            try
            {
                _hub.Publish(new RunStartedEvent(this));
            }
            finally
            {
                _hub.Publish(new RunFinishedEvent(this));
            }
        }
    }
}