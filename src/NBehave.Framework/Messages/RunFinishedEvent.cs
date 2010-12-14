namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class RunFinishedEvent : TinyMessageBase
    {
        public RunFinishedEvent(object sender)
            : base(sender)
        {
        }
    }
}