namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    using Gherkin;

    using NBehave.Narrator.Framework.Tiny;

    public class ParsedTable : GenericTinyMessage<IList<IList<Token>>>
    {
        public ParsedTable(object sender, IList<IList<Token>> titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}