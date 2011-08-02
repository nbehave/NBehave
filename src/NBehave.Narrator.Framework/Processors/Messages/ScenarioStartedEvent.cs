using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class ScenarioStartedEvent : GenericTinyMessage<Scenario>
    {
        public ScenarioStartedEvent(object sender, Scenario content)
            : base(sender, content)
        {
        }
    }
}