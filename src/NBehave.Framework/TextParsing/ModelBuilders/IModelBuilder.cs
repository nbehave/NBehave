namespace NBehave.Narrator.Framework.Processors
{
    using NBehave.Narrator.Framework.Tiny;

    public interface IModelBuilder
    {
    }

    internal class ScenarioBuilt : GenericTinyMessage<Scenario>
    {
        public ScenarioBuilt(object sender, Scenario scenario)
            : base(sender, scenario)
        {
        }
    }

    internal class FeatureBuilt : GenericTinyMessage<Feature>
    {
        public FeatureBuilt(object sender, Feature titleAndNarrative)
            : base(sender, titleAndNarrative)
        {
        }
    }
}