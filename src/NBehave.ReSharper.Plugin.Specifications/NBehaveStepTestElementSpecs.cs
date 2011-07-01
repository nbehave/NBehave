using NBehave.ReSharper.Plugin.UnitTestProvider;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public class NBehaveStepTestElementSpecs
    {
        [Test]
        public void Should_consider_same_instance_equal()
        {
            var featureFile = ResharperStubs.StubFeatureFile();
            var parentParent = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var parent = new NBehaveScenarioTestElement("scenario title", ResharperStubs.StubFeatureFile(), null, null, parentParent);
            var p = new NBehaveStepTestElement("Given something", featureFile, null, null, parent);
            Assert.AreEqual(p, p);
        }

        [Test]
        public void Should_consider_different_instances_with_same_title_equal()
        {
            var featureFileA = ResharperStubs.StubFeatureFile();
            var parentParentA = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var parentA = new NBehaveScenarioTestElement("scenario title", ResharperStubs.StubFeatureFile(), null, null, parentParentA);
            var a = new NBehaveStepTestElement("Given something", featureFileA, null, null, parentA);

            var featureFileB = ResharperStubs.StubFeatureFile();
            var parentParentB = new NBehaveFeatureTestElement("feature title", ResharperStubs.StubFeatureFile(), null, null);
            var parentB = new NBehaveScenarioTestElement("scenario title", ResharperStubs.StubFeatureFile(), null, null, parentParentB);
            var b = new NBehaveStepTestElement("Given something", featureFileB, null, null, parentB);
            Assert.AreEqual(a, b);
        }
    }
}