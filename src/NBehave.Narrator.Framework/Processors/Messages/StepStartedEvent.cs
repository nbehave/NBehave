using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class StepStartedEvent : GenericTinyMessage<StringStep>
    {
        public StepStartedEvent(object sender, StringStep content)
            : base(sender, content)
        {
        }
    }
}