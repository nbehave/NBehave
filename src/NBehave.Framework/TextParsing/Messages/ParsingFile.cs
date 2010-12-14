namespace NBehave.Narrator.Framework
{
    using System;

    using NBehave.Narrator.Framework.Tiny;

    public class ParsingFile : GenericTinyMessage<string>
    {
        public ParsingFile(object sender, string scenario)
            : base(sender, scenario)
        {
        }
    }
}