namespace NBehave.Narrator.Framework.EventListeners
{
    public class NullEventListener : IEventListener
    {
         void IEventListener.FeatureCreated(string feature)
        {
        }

        void IEventListener.FeatureNarrative(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {            
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.RunFinished()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult result)
        {            
        }

    }
}
