namespace NBehave.Narrator.Framework.Contracts
{
    using System.Collections.Generic;

    using NBehave.Narrator.Framework.Tiny;

    public class ScenarioFilesLoaded : GenericTinyMessage<IEnumerable<string>>
    {
        public ScenarioFilesLoaded(object sender, IEnumerable<string> content)
            : base(sender, content)
        {
        }
    }
}