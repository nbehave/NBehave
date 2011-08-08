using System.Collections.Generic;
using NBehave.Gherkin;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class ParsedTable : GenericTinyMessage<IList<IList<Token>>>
    {
        public ParsedTable(object sender, IList<IList<Token>> titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}