using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class ScenarioFinishedEvent : GenericTinyMessage<Scenario>
    {
        public ScenarioFinishedEvent(object sender, Scenario scenario) 
            : base(sender, scenario)
        {
        }
    }
}