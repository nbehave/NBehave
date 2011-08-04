using System.Linq;
using NBehave.Fluent.Framework.Extensions;
using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace NBehave.Fluent.Framework.Specifications
{
    [TestFixture]
    public class ScenarioDrivenSpecBaseSpecs_WithActionSteps : ScenarioDrivenSpecBase
    {
        protected override Feature CreateFeature()
        {
            return new Feature("ScenarioDrivenSpecBase");
        }

        [SetUp]
        public override void MainSetup()
        {
            base.MainSetup();
        }

        [Test]
        public void PendingStepsAreDetected()
        {
            Feature.AddScenario("Detect pending steps")
                .WithHelperObject<ScenarioDrivenSpecBaseSpecSteps>()
                .Given("A step pending implementation")
                .When("The spec is executed")
                .Then("The step should be identified")
                ;

            Assert.AreEqual(3, Feature.FindPendingSteps()
                                      .First()
                                      .Count());
        }
    }

    [ActionSteps]
    public class ScenarioDrivenSpecBaseSpecSteps
    {
    }
}
