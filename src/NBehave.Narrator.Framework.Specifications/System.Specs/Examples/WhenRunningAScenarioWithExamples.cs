using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningAScenarioWithScenarioOutlines : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] {"NBehave.Narrator.Framework.Specifications.dll"})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {@"System.Specs\Examples\Examples.feature"});
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(1));
        }
    }

    [ActionSteps]
    public class ScenarioOutlineSteps
    {
        [Given("this scenario containing examples $col1")]
        public void Given(int col1)
        {
        }

        [When("the scenario is executed $col2")]
        public void When(int col2)
        {
        }

        [Then("it should be templated and executed with each $row")]
        public void Then(int row)
        {
        }
    }
}