namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsedFeature : GenericTinyMessage<string>
    {
        public ParsedFeature(object sender, string titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}