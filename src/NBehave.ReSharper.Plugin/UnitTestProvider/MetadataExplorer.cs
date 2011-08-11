using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class MetadataExplorer
    {
        private readonly TestProvider _testProvider;
        private readonly UnitTestElementConsumer _consumer;
        private readonly IProject _project;
        private readonly ProjectModelElementEnvoy _projectModel;
        private readonly IFeatureRunner _featureRunner;
        private readonly ISolution _solution;
        private readonly ITinyMessengerHub _hub;

        public MetadataExplorer(TestProvider provider, ISolution solution, IProject project, UnitTestElementConsumer consumer)
        {
            Initialiser.Initialise();
            _testProvider = provider;
            _consumer = consumer;
            _project = project;
            _solution = solution;
            _projectModel = new ProjectModelElementEnvoy(_project);
            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            _featureRunner = TinyIoCContainer.Current.Resolve<IFeatureRunner>();
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject()
                .Select(_ => _.Location.FullPath)
                .ToList();

            IEnumerable<Feature> features = null;
            var featuresLoadedSubscription = _hub.Subscribe<FeaturesLoaded>(_ => features = _.Content);
            try
            {
                _featureRunner.DryRun(featureFiles);
                var elements = new FeatureMapper(_testProvider, _projectModel, _solution).MapFeatures(features);
                BindFeatures(elements);
            }
            finally
            {
                _hub.Unsubscribe<FeaturesLoaded>(featuresLoadedSubscription);
            }
        }

        private IEnumerable<IProjectFile> GetFeatureFilesFromProject()
        {
            var validExtensions = NBehaveConfiguration.FeatureFileExtensions;
            var featureFiles = _project
                .GetAllProjectFiles()
                .Where(_ => validExtensions.Any(e => e.Equals(Path.GetExtension(_.Name), StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
            return featureFiles;
        }

        private void BindFeatures(IEnumerable<NBehaveUnitTestElementBase> features)
        {
            foreach (var feature in features)
                _consumer(feature);
        }
    }
}