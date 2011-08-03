using System.IO;
using NBehave.Narrator.Framework.Processors;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningAScenarioWithoutAFeature : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;
        private ScenarioMustHaveFeatureException _exception;

        protected override void EstablishContext()
        {
            var asm = Path.GetFileName(GetType().Assembly.Location);
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { Path.GetFileName(GetType().Assembly.Location) })
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Scenarios\ScenarioWithoutFeature.feature" });
        }

        protected override void Because()
        {
            try
            {
                _results = _config.Build().Run();
            }
            catch (ScenarioMustHaveFeatureException exception)
            {
                _exception = exception;
            }
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(_exception, Is.Not.Null);
        }
    }

    [ActionSteps]
    public class ScenarioStepsWithoutFeature
    {
        [Given("this featureless scenario")]
        public void Given()
        {
        }

        [When("this featureless scenario is executed")]
        public void When()
        {
        }

        [Then("the user should be notified that this is not valid")]
        public void Then()
        {
        }
    }
}