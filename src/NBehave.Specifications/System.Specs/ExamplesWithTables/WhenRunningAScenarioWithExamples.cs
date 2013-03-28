using NBehave.Extensions;
using NUnit.Framework;
using Should;

namespace NBehave.Specifications.System.Specs.ExamplesWithTables
{
    [TestFixture]
    public class WhenRunningAScenarioWithExamplesAndTables : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
           Configure_With(@"System.Specs\ExamplesWithTables\ExamplesWithTables.feature");
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
    public class ExamplesWithTableSteps
    {
        private int _left;
        private int _right;

        [Given("this scenario containing scenario outline and a table:")]
        public void Given(int left, int right)
        {
            _left = left;
            _right = right;
        }

        [When("the tabled scenario outline is executed")]
        public void When()
        {
        }

        [Then("the table should be templated into the scenario outline and executed with each row:")]
        public void Then(int sum)
        {
            sum.ShouldEqual(_left + _right);
        }
    }
}