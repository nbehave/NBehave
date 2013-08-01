using NBehave.ReSharper.Plugin.UnitTestProvider;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture, Ignore("Fix!")]
    public class NBehaveFeatureTestElementSpecs
    {
        [Test]
        public void Should_consider_same_instance_equal()
        {
            var featureFile = ResharperStubs.StubFeatureFile();
            var p = new NBehaveFeatureTestElement("feature title", featureFile, null, null);
            Assert.AreEqual(p, p);
        }

        [Test]
        public void Should_consider_different_instances_with_same_title_equal()
        {
            var featureFileA = ResharperStubs.StubFeatureFile();
            var a = new NBehaveFeatureTestElement("feature title", featureFileA, null, null);
            var featureFileB = ResharperStubs.StubFeatureFile();
            var b = new NBehaveFeatureTestElement("feature title", featureFileB, null, null);
            Assert.AreEqual(a, b);
        }
    }
}