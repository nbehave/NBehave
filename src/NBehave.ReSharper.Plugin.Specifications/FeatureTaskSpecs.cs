using NBehave.ReSharper.Plugin.UnitTestRunner;
using NUnit.Framework;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public class AssemblyTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var a = new AssemblyTask(@"X:\Project\bin\debug\project.dll");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var a = new AssemblyTask(@"X:\Project\bin\debug\project.dll");
            var b = new AssemblyTask(@"X:\Project\bin\debug\project.dll");
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture]
    public class FeatureTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var a = new FeatureTask(@"X:\Project\someFeature.feature");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var a = new FeatureTask(@"X:\Project\someFeature.feature");
            var b = new FeatureTask(@"X:\Project\someFeature.feature");
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture]
    public class NBehaveScenarioTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var a = new NBehaveScenarioTask(@"X:\Project\someFeature.feature", "scenario");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var a = new NBehaveScenarioTask(@"X:\Project\someFeature.feature", "scenario");
            var b = new NBehaveScenarioTask(@"X:\Project\someFeature.feature", "scenario");
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture]
    public class NBehaveStepTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var a = new NBehaveStepTask(@"X:\Project\someFeature.feature", "scenario", "step");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var a = new NBehaveStepTask(@"X:\Project\someFeature.feature", "scenario", "step");
            var b = new NBehaveStepTask(@"X:\Project\someFeature.feature", "scenario", "step");
            Assert.AreEqual(a, b);
        }
    }
}