using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Gherkin;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;
using NBehave.Narrator.Framework.TextParsing;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class MetadataExplorer
    {
        private readonly IUnitTestProvider testProvider;
        private readonly UnitTestElementConsumer consumer;
        private readonly IProject project;
        private readonly ProjectModelElementEnvoy projectModel;
        private readonly ISolution solution;

        public MetadataExplorer(IUnitTestProvider provider, ISolution solution, IProject project, UnitTestElementConsumer consumer)
        {
            testProvider = provider;
            this.consumer = consumer;
            this.project = project;
            this.solution = solution;
            projectModel = new ProjectModelElementEnvoy(this.project);
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject()
                .Select(_ => _.Location.FullPath)
                .ToList();

            var features = ParseFeatures(featureFiles, NBehaveConfiguration.New);
            var elements = new FeatureMapper(testProvider, projectModel, solution).MapFeatures(features);
            BindFeatures(elements);
        }

        private IEnumerable<Feature> ParseFeatures(IEnumerable<string> featureFiles, NBehaveConfiguration configuration)
        {
            var features = new List<Feature>();
            EventHandler<EventArgs<Feature>> featureStarted = (s, e) => features.Add(e.EventInfo);

            var parser = new GherkinScenarioParser(configuration);
            parser.FeatureEvent += featureStarted;
            foreach (var featureFile in featureFiles)
            {
                try
                {
                    parser.Parse(featureFile);
                }
                catch (ParseException)
                {

                }
            }
            parser.FeatureEvent -= featureStarted;
            return features;
        }

        private IEnumerable<IProjectFile> GetFeatureFilesFromProject()
        {
            var validExtensions = NBehaveConfiguration.FeatureFileExtensions;
            var featureFiles = project
                .GetAllProjectFiles()
                .Where(_ => validExtensions.Any(e => e.Equals(Path.GetExtension(_.Name), StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
            return featureFiles;
        }

        private void BindFeatures(IEnumerable<NBehaveUnitTestElementBase> features)
        {
            foreach (var feature in features)
                consumer(feature);
        }
    }
}