using NBehave.ReSharper.Plugin.UnitTestProvider;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public class NBehaveBackgroundScenarioTestElementSpecs
    {
        [Test]
        public void Should_consider_same_instance_equal()
        {
            var featureFile = ResharperStubs.StubFeatureFile();
            var parent = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var p1 = new NBehaveScenarioTestElement("scenario title", featureFile, null, null, parent);
            var p = new NBehaveBackgroundTestElement("scenario title", featureFile, null, null, p1);
            Assert.AreEqual(p, p);
        }

        [Test]
        public void Should_not_consider_scenario_and_background_equal()
        {
            var featureFile = ResharperStubs.StubFeatureFile();
            var parent = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var p1 = new NBehaveScenarioTestElement("scenario title", featureFile, null, null, parent);
            var p = new NBehaveBackgroundTestElement("scenario title", featureFile, null, null, p1);
            Assert.AreNotEqual(p1, p);
            Assert.AreNotEqual(p, p1);
        }

        [Test]
        public void Should_consider_different_instances_with_same_title_equal()
        {
            var featureFileA = ResharperStubs.StubFeatureFile();
            var parentA = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var a1 = new NBehaveScenarioTestElement("scenario title", featureFileA, null, null, parentA);
            var a = new NBehaveBackgroundTestElement("scenario title", featureFileA, null, null, a1);

            var parentB = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var featureFileB = ResharperStubs.StubFeatureFile();
            var b1 = new NBehaveScenarioTestElement("scenario title", featureFileB, null, null, parentB);
            var b = new NBehaveBackgroundTestElement("scenario title", featureFileB, null, null, b1);
            Assert.AreEqual(a, b);
        }
    }
}