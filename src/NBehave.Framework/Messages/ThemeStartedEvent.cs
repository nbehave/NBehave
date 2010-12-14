namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ThemeStartedEvent : GenericTinyMessage<string>
    {
        public ThemeStartedEvent(object sender, string content)
            : base(sender, content)
        {
        }
    }
}