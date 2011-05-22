namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsedBackground : GenericTinyMessage<string>
    {
        public ParsedBackground(object sender, string titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}