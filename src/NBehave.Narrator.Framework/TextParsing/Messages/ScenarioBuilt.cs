namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioBuilt : GenericTinyMessage<Scenario>
    {
        public ScenarioBuilt(object sender, Scenario scenario)
            : base(sender, scenario)
        {
        }
    }
}