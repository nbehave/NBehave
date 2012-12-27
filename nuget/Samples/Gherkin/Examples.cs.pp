using NBehave;
using NBehave.Spec.NUnit;

namespace $rootnamespace$.Gherkin
{
    [ActionSteps]
    public class ExamplesSteps
    {
        int cucumbers;

        [Given("there are $start cucumbers")]
        public void GivenCucumbers(int start)
        {
            cucumbers = start;
        }

        [When("I eat $x cucumbers")]
        public void EatCucumbers(int x)
        {
            cucumbers -= x;
        }

        [Then("I should have $y cucumbers")]
        public void CucumbersLeft(int y)
        {
            cucumbers.ShouldEqual(y);
        }
    }
}