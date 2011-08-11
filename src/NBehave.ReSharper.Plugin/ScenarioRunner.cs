using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;

        public ScenarioRunner(NBehaveConfiguration configuration, ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            _configuration = configuration;
            _hub = hub;
            _stringStepRunner = stringStepRunner;
        }

        public void Run(Feature feature)
        {
            if (_configuration.IsDryRun)
                return;
            DoRun(feature);
        }

        private void DoRun(Feature feature)
        {
            var runner = new Narrator.Framework.Processors.ScenarioRunner(_hub, _stringStepRunner);
            runner.Run(feature);
        }
    }
}