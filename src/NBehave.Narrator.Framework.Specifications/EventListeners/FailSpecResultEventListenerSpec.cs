using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class FailSpecResultEventListenerSpec
    {
        [Test]
        public void Should_not_throw_if_feature_passes()
        {
            var listener = new FailSpecResultEventListener();
            var runner = NBehaveConfiguration
                .New
                .SetScenarioFiles(new[] { TestFeatures.FeatureNamedStory })
                .SetAssemblies(new[] { "TestPlainTextAssembly.dll" })
                .SetEventListener(listener)
                .Build(); Assert.DoesNotThrow(() => runner.Run());
        }

        [Test]
        public void Should_throw_TestFailedException()
        {
            var listener = new FailSpecResultEventListener();
            var runner = NBehaveConfiguration
                .New
                .SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                .SetAssemblies(new[] { "TestPlainTextAssembly.dll" })
                .SetEventListener(listener)
                .Build();
            Assert.Throws<StepFailedException>(() => runner.Run());
        }
    }
}