namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioCreated : GenericTinyMessage<ScenarioWithSteps>
    {
        public ScenarioCreated(object sender, ScenarioWithSteps content)
            : base(sender, content)
        {
        }
    }
}