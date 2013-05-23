using NBehave.ReSharper.Plugin.UnitTestProvider;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture, Ignore("Fix!")]
    public class NBehaveScenarioTestElementSpecs
    {
        [Test]
        public void Should_consider_same_instance_equal()
        {
            var featureFile = ResharperStubs.StubFeatureFile();
            var parent = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var p = new NBehaveScenarioTestElement("scenario title", featureFile, null, null, parent);
            Assert.AreEqual(p, p);
        }

        [Test]
        public void Should_consider_different_instances_with_same_title_equal()
        {
            var featureFileA = ResharperStubs.StubFeatureFile();
            var parentA = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var a = new NBehaveScenarioTestElement("scenario title", featureFileA, null, null, parentA);

            var parentB = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var featureFileB = ResharperStubs.StubFeatureFile();
            var b = new NBehaveScenarioTestElement("scenario title", featureFileB, null, null, parentB);
            Assert.AreEqual(a, b);
        }
    }
}