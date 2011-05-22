namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ParsingFileEnd : GenericTinyMessage<string>
    {
        public ParsingFileEnd(object sender, string scenario)
            : base(sender, scenario)
        {
        }
    }
}