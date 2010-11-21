namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class ThemeStarted : GenericTinyMessage<string>
    {
        public ThemeStarted(object sender, string content)
            : base(sender, content)
        {
        }
    }
}