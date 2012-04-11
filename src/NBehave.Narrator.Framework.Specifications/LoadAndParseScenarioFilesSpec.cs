using System.Collections.Generic;
using NBehave.Narrator.Framework.Internal;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class LoadAndParseScenarioFilesSpec
    {
        private LoadScenarioFiles loadScenarioFiles;
        private NBehaveConfiguration config;
        private ParseScenarioFiles parseScenarioFiles;
        private IEnumerable<Feature> features;

        private void CreateLoaderAndParser()
        {
            loadScenarioFiles = new LoadScenarioFiles(config);
            parseScenarioFiles = new ParseScenarioFiles(config);
            var files = loadScenarioFiles.LoadFiles();
            features = parseScenarioFiles.LoadFiles(files);
        }

        [Test]
        public void ShouldBeAbleToUseRelativePathsWithDots()
        {
            config = ConfigurationNoAppDomain.New.SetScenarioFiles(new[] { @".\*.feature" });

            CreateLoaderAndParser();
            CollectionAssert.IsNotEmpty(features);
        }
    }
}