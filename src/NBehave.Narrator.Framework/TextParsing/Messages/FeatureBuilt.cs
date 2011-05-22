namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    internal class FeatureBuilt : GenericTinyMessage<Feature>
    {
        public FeatureBuilt(object sender, Feature titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}