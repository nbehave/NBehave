namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureNarrativeEvent : GenericTinyMessage<string>
    {
        public FeatureNarrativeEvent(object sender, string content)
            : base(sender, content)
        {
        }
    }
}