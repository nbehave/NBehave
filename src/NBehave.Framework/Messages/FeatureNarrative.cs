namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureNarrative : GenericTinyMessage<string>
    {
        public FeatureNarrative(object sender, string content)
            : base(sender, content)
        {
        }
    }
}