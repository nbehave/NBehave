using NBehave.Narrator.Framework;

namespace NBehave.Spec.Extensions
{
    public static class FeatureExtensions
    {
        public static IAsAFragment AddStory(this Feature feature)
        {
            return new StoryBuilder.AsAFragment(feature);
        }

        public static IScenarioBuilderStartWithHelperObject AddScenario(this Feature feature)
        {
            return new ScenarioBuilder.StartFragment(feature);
        }
    }
}
