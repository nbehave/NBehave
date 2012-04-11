using System.Text;
using NBehave.Narrator.Framework.Extensions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Extensions
{
    [TestFixture]
    public class StoryRunnerFilterExtensionsSpec
    {
        [Test]
        public void Should_build_pattern_based_on_class_name()
        {
            var filter = In.Context<StoryRunnerFilterExtensionsSpec>();
            Assert.AreEqual("^StoryRunnerFilterExtensionsSpec$", filter.ClassNameFilter.ToString());
        }

        [Test]
        public void Should_build_pattern_based_on_class_name_using_global_context()
        {
            var filter = In.GlobalContext().And<StoryRunnerFilterExtensionsSpec>();
            Assert.AreEqual("^StoryRunnerFilterExtensionsSpec$", filter.ClassNameFilter.ToString());
        }

        [Test]
        public void Should_build_pattern_based_on_multiple_class_names()
        {
            var filter = In.Context<StoryRunnerFilterExtensionsSpec>().And<StringBuilder>().And<TestAttribute>();
            Assert.AreEqual("^StoryRunnerFilterExtensionsSpec|StringBuilder|TestAttribute$", filter.ClassNameFilter.ToString());
        }
    }
}
