namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioCreatedEvent : GenericTinyMessage<Scenario>
    {
        public ScenarioCreatedEvent(object sender, Scenario content)
            : base(sender, content)
        {
        }
    }
}