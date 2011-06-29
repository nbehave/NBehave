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
            Assert.DoesNotThrow(() =>
            {
                var listener = new FailSpecResultEventListener();
                NBehaveConfiguration
                    .New
                    .SetScenarioFiles(new[] { TestFeatures.FeatureNamedStory })
                    .SetAssemblies(new[] { "TestPlainTextAssembly.dll" })
                    .SetEventListener(listener)
                    .Run();
            });
        }

        [Test]
        public void Should_throw_TestFailedException()
        {
            Assert.Throws<StepFailedException>(() =>
                                                   {
                                                       var listener = new FailSpecResultEventListener();
                                                       NBehaveConfiguration
                                                           .New
                                                           .SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                                                           .SetAssemblies(new[] { "TestPlainTextAssembly.dll" })
                                                           .SetEventListener(listener)
                                                           .Run();
                                                   });
        }
    }
}