using JetBrains.ProjectModel;
using JetBrains.Util;
using NBehave.ReSharper.Plugin.UnitTestRunner;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture, Ignore("Fix!")]
    public class AssemblyTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var a = new NBehaveAssemblyTask(@"X:\Project\bin\debug\project.dll");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var a = new NBehaveAssemblyTask(@"X:\Project\bin\debug\project.dll");
            var b = new NBehaveAssemblyTask(@"X:\Project\bin\debug\project.dll");
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture, Ignore("Fix!")]
    public class FeatureTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var projFile = MockRepository.GenerateStub<IProjectFile>();
            projFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveFeatureTask("someFeature", projFile);
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var aFile = MockRepository.GenerateStub<IProjectFile>();
            aFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveFeatureTask("someFeature", aFile);
            var bFile = MockRepository.GenerateStub<IProjectFile>();
            bFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var b = new NBehaveFeatureTask("someFeature", bFile);
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture, Ignore("Fix!")]
    public class NBehaveScenarioTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var projFile = MockRepository.GenerateStub<IProjectFile>();
            projFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveScenarioTask(projFile, "scenario");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var aFile = MockRepository.GenerateStub<IProjectFile>();
            aFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveScenarioTask(aFile, "scenario");
            var bFile = MockRepository.GenerateStub<IProjectFile>();
            bFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var b = new NBehaveScenarioTask(bFile, "scenario");
            Assert.AreEqual(a, b);
        }
    }

    [TestFixture, Ignore("Fix!")]
    public class NBehaveStepTaskSpecs
    {
        [Test]
        public void Should_see_same_instance_as_equal()
        {
            var projFile = MockRepository.GenerateStub<IProjectFile>();
            projFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveStepTask(projFile, "scenario", "step");
            Assert.AreEqual(a, a);
        }

        [Test]
        public void Should_see_two_different_instances_with_same_feature_as_equal()
        {
            var aFile = MockRepository.GenerateStub<IProjectFile>();
            aFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var a = new NBehaveStepTask(aFile, "scenario", "step");
            var bFile = MockRepository.GenerateStub<IProjectFile>();
            bFile.Stub(_ => _.Location).Return(FileSystemPath.Parse(@"X:\Project\someFeature.feature"));
            var b = new NBehaveStepTask(bFile, "scenario", "step");
            Assert.AreEqual(a, b);
        }
    }
}