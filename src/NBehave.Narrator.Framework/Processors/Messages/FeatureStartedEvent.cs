using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class FeatureStartedEvent : GenericTinyMessage<string>
    {
        public FeatureStartedEvent(object sender, string content)
            : base(sender, content)
        {
        }
    }
}