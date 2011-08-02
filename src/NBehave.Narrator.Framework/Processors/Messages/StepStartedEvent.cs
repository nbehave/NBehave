using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class StepStartedEvent : GenericTinyMessage<string>
    {
        public StepStartedEvent(object sender, string content)
            : base(sender, content)
        {
        }
    }
}