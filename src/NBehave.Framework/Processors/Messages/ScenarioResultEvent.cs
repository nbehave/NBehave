namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioResultEvent : GenericTinyMessage<ScenarioResult>
    {
        public ScenarioResultEvent(object sender, ScenarioResult content)
            : base(sender, content)
        {
        }
    }
}