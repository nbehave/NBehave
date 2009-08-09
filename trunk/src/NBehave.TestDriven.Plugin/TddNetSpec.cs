using NBehave.Narrator.Framework;

namespace NBehave.TestDriven.Plugin
{
    [Theme("TDD.Net integration")]
    public class TddNetSpec
    {
        [Story]
        public void Implementing_NBehave_support_in_testdriven_dot_net()
        {
            var tddNetStory = new Story("Testdriven.NET Support");

            tddNetStory
                .AsA("fan of NBehave")
                .IWant("NBehave to work with Testdriven.NET")
                .SoThat("I get less friction developing code using NBehave");

            tddNetStory
                .WithScenario("VS integration - right click method")
                .Pending("Pending test")
                .Given("A story in a C# source file", () => { })
                .And("User have right clicked on a method in code window", () => { })
                .And("Left clicked 'Run Test(s)'", () => { })
                .When("Testdriven.Net runs story", () => { })
                .Then("NBehave framework should be invoked", () => { })
                .And("Testdriven.Net should be notified of the result", () => { });

            tddNetStory
                .WithScenario("VS integration - right click .cs file")
                .Given("A story in a C# source file", () => { })
                .And("User have right clicked on file", () => { })
                .And("Left clicked 'Run Test(s)'", () => { })
                .When("Testdriven.Net runs story", () => { })
                .Then("NBehave framework should be invoked", () => { })
                .And("Testdriven.Net should be notified of the result", () => { });
        }

        [Story]
        public void SomeOtherStory()
        {
            var tddNetStory = new Story("Testdriven.NET Support second story");

            tddNetStory
                .AsA("addict of NBehave")
                .IWant("NBehave to work with Testdriven.NET")
                .SoThat("I get my fix");

            tddNetStory
                .WithScenario("VS integration - something")
                .Given("A story in a C# source file", () => { })
                .When("Testdriven.Net runs story", () => { })
                .Then("NBehave framework should be invoked", () => { })
                .And("Testdriven.Net should be notified of the result", () => { });
        }

    }
}
