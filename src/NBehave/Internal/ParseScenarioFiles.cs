using System.Collections.Generic;
using NBehave.Configuration;
using NBehave.Domain;
using NBehave.TextParsing;

namespace NBehave.Internal
{
    public class ParseScenarioFiles
    {
        private readonly NBehaveConfiguration configuration;

        public ParseScenarioFiles(NBehaveConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<Feature> LoadFiles(IEnumerable<string> files)
        {
            var textParser = new GherkinScenarioParser(configuration);
            var features = new List<Feature>();
            Feature feature = null;
            textParser.FeatureEvent += (s, e) =>
                                           {
                                               feature = e.EventInfo;
                                               features.Add(e.EventInfo);
                                           };
            textParser.ScenarioEvent += (s, e) =>
                                            {
                                                if (feature == null)
                                                {
                                                    feature = e.EventInfo.Feature;
                                                    features.Add(feature);
                                                }
                                            };
            foreach (var file in files)
            {
                feature = null;
                textParser.Parse(file);
            }
            return features;
        }
    }
}