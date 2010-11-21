namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioResultMessage : GenericTinyMessage<ScenarioResult>
    {
        public ScenarioResultMessage(object sender, ScenarioResult content)
            : base(sender, content)
        {
        }
    }
}