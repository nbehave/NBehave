namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureCreated : GenericTinyMessage<string>
    {
        public FeatureCreated(object sender, string content) 
            : base(sender, content)
        {
        }
    }
}