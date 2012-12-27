namespace NBehave.TextParsing.ModelBuilders
{
    public class FeatureBuilder
    {
        Feature feature;

        public FeatureBuilder(IGherkinParserEvents gherkinParser)
        {
            gherkinParser.FeatureEvent += (s, e) => { feature = e.EventInfo; };
            gherkinParser.ScenarioEvent += (s, e) =>
                                               {
                                                   CreateFeatureIfNull(e.EventInfo.Source);
                                                   feature.AddScenario(e.EventInfo);
                                               };
            gherkinParser.BackgroundEvent += (s, e) =>
                                                 {
                                                     CreateFeatureIfNull(e.EventInfo.Source);
                                                     feature.AddBackground(e.EventInfo);
                                                 };
            gherkinParser.EofEvent += (s, e) => { feature = null; };
        }

        private void CreateFeatureIfNull(string source)
        {
            if (feature == null)
                feature = new Feature("", source);
        }
    }
}