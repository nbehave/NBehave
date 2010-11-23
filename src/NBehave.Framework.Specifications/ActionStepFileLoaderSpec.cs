using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    using NBehave.Narrator.Framework.Contracts;
    using NBehave.Narrator.Framework.Tiny;

    [TestFixture]
    public class LoadScenarioFilesSpec
    {
        private LoadScenarioFiles _actionStepFileLoader;

        [SetUp]
        public void EstablishContext()
        {
            var config = NBehaveConfiguration.New;
            _actionStepFileLoader = new LoadScenarioFiles(config, TinyIoCContainer.Current.Resolve<ITinyMessengerHub>(),
                new StringStepRunner(new ActionCatalog()));
        }

        [Test]
        public void ShouldTreatEachFileAsAStory()
        {
            var files = new[]
            {
                TestFeatures.ScenariosWithoutFeature,
                TestFeatures.ScenarioWithNoActionSteps
            };
            var stories = _actionStepFileLoader.Load(files);
            Assert.That(stories.Count, Is.EqualTo(2));
        }

        [Test]
        public void ShouldHaveSourceSetOnStep()
        {
            var files = new[]
            {
                TestFeatures.ScenariosWithoutFeature,
            };
            var stories = _actionStepFileLoader.Load(files);

            Assert.That(stories[0].Scenarios.First().Steps.First().Source, Is.Not.Null);
            Assert.That(stories[0].Scenarios.First().Steps.First().Source, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void ShouldBeAbleToUseRelativePathsWithDots()
        {
            IEnumerable<string> locations = new[] { @"..\*.*" };
            var steps = _actionStepFileLoader.Load(locations);
            Assert.That(steps, Is.Not.Null);
        }
    }
}