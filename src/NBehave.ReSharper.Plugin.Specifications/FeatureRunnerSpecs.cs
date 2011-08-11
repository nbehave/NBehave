using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;
using NBehave.ReSharper.Plugin.UnitTestProvider;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public class FeatureRunnerSpecs
    {
        private IFeatureRunner _featureRunner;
        private IEnumerable<Narrator.Framework.Feature> _features;

        [SetUp]
        public void SetUp()
        {
            Initialiser.Initialise();
            var hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            hub.Subscribe<FeaturesLoaded>(_ => _features = _.Content);
            _featureRunner = new FeatureRunner();
            _featureRunner.DryRun(new[] { Feature.Scenario });
        }

        [Test]
        public void Should_have_one_feature()
        {
            Assert.That(_features.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_have_two_scenarios()
        {
            Assert.That(_features.First().Scenarios.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_have_three_steps_in_each_scenario()
        {
            foreach (var scenario in _features.First().Scenarios)
            {
                Assert.That(scenario.Steps.Count(), Is.EqualTo(3));
            }
        }
    }
}