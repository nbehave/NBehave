using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class MetadataExplorer
    {
        private readonly IUnitTestProvider testProvider;
        private readonly UnitTestElementConsumer consumer;
        private readonly IProject project;
        private readonly ProjectModelElementEnvoy projectModel;
        private readonly IFeatureRunner featureRunner;
        private readonly ISolution solution;
        private readonly IRunContextEvents contextEvents;

        public MetadataExplorer(IUnitTestProvider provider, ISolution solution, IProject project, UnitTestElementConsumer consumer)
        {
            Initialiser.Initialise();
            testProvider = provider;
            this.consumer = consumer;
            this.project = project;
            this.solution = solution;
            projectModel = new ProjectModelElementEnvoy(this.project);
            featureRunner = TinyIoCContainer.Current.Resolve<IFeatureRunner>();
            contextEvents = TinyIoCContainer.Current.Resolve<IRunContextEvents>();
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject()
                .Select(_ => _.Location.FullPath)
                .ToList();

            var features = new List<Feature>();
            EventHandler<EventArgs<Feature>> featureStarted = (s, e) => features.Add(e.EventInfo);
            contextEvents.OnFeatureStarted += featureStarted;
            try
            {
                featureRunner.DryRun(featureFiles);
                var elements = new FeatureMapper(testProvider, projectModel, solution).MapFeatures(features);
                BindFeatures(elements);
            }
            finally
            {
                contextEvents.OnFeatureStarted -= featureStarted;
            }
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