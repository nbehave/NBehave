namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsedScenario : GenericTinyMessage<string>
    {
        public ParsedScenario(object sender, string scenarioTitle)
            : base(sender, scenarioTitle)
        {
        }
    }
}