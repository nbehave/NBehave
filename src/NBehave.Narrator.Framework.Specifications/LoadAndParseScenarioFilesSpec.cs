using System.Collections.Generic;
using NBehave.Narrator.Framework.Contracts;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class LoadAndParseScenarioFilesSpec
    {
        private LoadScenarioFiles _loadScenarioFiles;
        private NBehaveConfiguration _config;
        private ITinyMessengerHub _hub;
        private ParseScenarioFiles _parseScenarioFiles;
        private IEnumerable<Feature> _features;

        private void CreateLoaderAndParser()
        {
            TinyIoCContainer.Current.Register<ITinyMessengerHub, TinyMessengerHub>();
            TinyIoCContainer.Current.RegisterMany<IModelBuilder>().AsSingleton();
            TinyIoCContainer.Current.Resolve<IEnumerable<IModelBuilder>>();

            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            _loadScenarioFiles = new LoadScenarioFiles(_config, _hub);
            _parseScenarioFiles = new ParseScenarioFiles(_hub, _config);
            _hub.Subscribe<FeaturesLoaded>(loaded => _features = loaded.Content);
            _loadScenarioFiles.Initialise();
        }

        [Test]
        public void ShouldBeAbleToUseRelativePathsWithDots()
        {
            _config = ConfigurationNoAppDomain.New.SetScenarioFiles(new[]
                                                                    {
                                                                        @"..\*.*"
                                                                    });

            CreateLoaderAndParser();

            Assert.That(_features, Is.Not.Null);
        }
    }
}