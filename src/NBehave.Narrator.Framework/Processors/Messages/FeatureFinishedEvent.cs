using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class FeatureFinishedEvent : GenericTinyMessage<string>
    {
        public FeatureFinishedEvent(object sender, string content) 
            : base(sender, content)
        {
        }
    }
}