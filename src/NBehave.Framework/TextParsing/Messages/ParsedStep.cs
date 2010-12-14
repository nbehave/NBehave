namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsedStep : GenericTinyMessage<string>
    {
        public ParsedStep(object sender, string stepText)
            : base(sender, stepText)
        {
        }
    }
}