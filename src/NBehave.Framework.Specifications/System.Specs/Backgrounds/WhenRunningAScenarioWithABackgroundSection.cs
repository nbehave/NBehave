namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenarioWithABackgroundSection : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Backgrounds\Background.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(this._results.NumberOfPassingScenarios, Is.EqualTo(1)); 
        }
    }

    [ActionSteps]
    public class ScenarioStepsForBackground
    {
        private int callCount = 0;

        [Given("this background section declaration")]
        public void FirstGiven()
        {
            Assert.That(callCount, Is.EqualTo(0));
            callCount++;
        }

        [Given("this one")]
        public void SecondGiven()
        {
            Assert.That(callCount, Is.EqualTo(1));
            callCount++;
        }

        [Given("this scenario under the context of a background section")]
        public void ThirdGiven()
        {
            Assert.That(callCount, Is.EqualTo(2));
            callCount++;
        }

        [When("the scenario is executed")]
        public void When()
        {
            Assert.That(callCount, Is.EqualTo(3));
            callCount++;
        }

        [Then("it should pass")]
        public void Then()
        {
            Assert.That(callCount, Is.EqualTo(4));
            callCount++;
        }

        [Then("the background section steps should be called before this scenario")]
        public void AnotherThen()
        {
            Assert.That(callCount, Is.EqualTo(5));
            callCount++;
        }
    }
}