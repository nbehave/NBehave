namespace NBehave.Narrator.Framework
{
    using System;

    using NBehave.Narrator.Framework.Tiny;

    public class ParsingFileStart : GenericTinyMessage<string>
    {
        public ParsingFileStart(object sender, string scenario)
            : base(sender, scenario)
        {
        }
    }
}