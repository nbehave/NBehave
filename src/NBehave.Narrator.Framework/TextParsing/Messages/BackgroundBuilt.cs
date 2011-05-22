using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    internal class BackgroundBuilt : GenericTinyMessage<Scenario>
    {
        public BackgroundBuilt(object sender, Scenario scenario)
            : base(sender, scenario)
        {
        }
    }
}