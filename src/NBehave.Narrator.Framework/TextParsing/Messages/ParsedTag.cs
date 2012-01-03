using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class ParsedTag : GenericTinyMessage<string>
    {
        private static readonly char[] at = new[] { '@' };

        public ParsedTag(object sender, string tag)
            : base(sender, tag.TrimStart(at))
        {
        }
    }
}