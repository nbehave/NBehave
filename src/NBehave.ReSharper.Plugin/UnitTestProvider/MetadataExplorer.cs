using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class MetadataExplorer
    {
        private readonly TestProvider _testProvider;
        private readonly UnitTestElementConsumer _consumer;
        private readonly IProject _project;
        private readonly ProjectModelElementEnvoy _projectEnvoy;
        private GherkinFileParser _gherkinParser;

        public MetadataExplorer(TestProvider provider, IProject project, UnitTestElementConsumer consumer)
        {
            _testProvider = provider;
            _consumer = consumer;
            _project = project;
            _projectEnvoy = new ProjectModelElementEnvoy(_project);
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject().ToList();
            _gherkinParser = new GherkinFileParser(_project.GetOutputAssemblyFile(), featureFiles, _testProvider, _projectEnvoy);
            var elements = _gherkinParser.ParseFilesToElements(featureFiles).ToList();
            BindFeatures(elements);
        }

        private IEnumerable<IProjectFile> GetFeatureFilesFromProject()
        {
            var validExtensions = new NBehaveConfiguration().FeatureFileExtensions;
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