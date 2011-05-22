namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    internal class ScenarioBuilt : GenericTinyMessage<Scenario>
    {
        public ScenarioBuilt(object sender, Scenario scenario)
            : base(sender, scenario)
        {
        }
    }
}