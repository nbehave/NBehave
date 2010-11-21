namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioCreated : GenericTinyMessage<string>
    {
        public ScenarioCreated(object sender, string content)
            : base(sender, content)
        {
        }
    }
}