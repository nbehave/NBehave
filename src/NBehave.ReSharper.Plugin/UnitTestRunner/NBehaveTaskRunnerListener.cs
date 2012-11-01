using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{

    public class NBehaveTaskRunnerListener : EventListener
    {
        private readonly ResharperResultPublisher publisher;

        public NBehaveTaskRunnerListener(ResharperResultPublisher publisher)
        {
            this.publisher = publisher;
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            publisher.Notify(result);
        }

        public override void FeatureFinished(FeatureResult result)
        {
            publisher.Notify(result);
        }
    }
}