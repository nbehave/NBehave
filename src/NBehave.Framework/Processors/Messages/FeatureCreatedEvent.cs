namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeatureCreatedEvent : GenericTinyMessage<string>
    {
        public FeatureCreatedEvent(object sender, string content) 
            : base(sender, content)
        {
        }
    }
}