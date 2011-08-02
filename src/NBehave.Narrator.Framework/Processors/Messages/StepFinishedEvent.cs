using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class StepFinishedEvent : GenericTinyMessage<string>
    {
        public StepFinishedEvent(object sender, string content)
            : base(sender, content)
        {
        }
    }
}