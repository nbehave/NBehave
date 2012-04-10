using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public class FeatureRunnerSpecs
    {
        private IFeatureRunner featureRunner;
        private FeatureResults results;

        [SetUp]
        public void SetUp()
        {
            Initialiser.Initialise();
            featureRunner = new FeatureRunner();
            results = featureRunner.DryRun(new[] { Feature.Scenario });
        }

        [Test]
        public void Should_have_one_feature()
        {
            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_have_two_scenarios()
        {
            Assert.That(results.First().ScenarioResults.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_have_three_steps_in_each_scenario()
        {
            foreach (var scenario in results.First().ScenarioResults)
                Assert.That(scenario.StepResults.Count(), Is.EqualTo(3));
        }
    }
}