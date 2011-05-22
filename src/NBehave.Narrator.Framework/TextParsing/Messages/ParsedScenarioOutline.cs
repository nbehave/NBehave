namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsedScenarioOutline : GenericTinyMessage<string>
    {
        public ParsedScenarioOutline(object sender, string titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}