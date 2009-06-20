namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    public class XmlOutputEventListenerTestData
    {
        [Theme("T1")]
        public class ThemeClass1
        {
            [Story]
            public void Story1()
            {
                Story story = new Story("S1");

                story
                    .AsA("X1")
                    .IWant("Y1")
                    .SoThat("Z1");

                story.WithScenario("SC1")
                    .Given("something", () => { })
                    .When("some event occurs", () => { })
                    .Then("there is some outcome", () => { });

                story.WithScenario("SC2")
                    .Given("something two", () => { })
                    .When("some event #2 occurs", () => { })
                    .Then("there is some outcome #2", () => { });

                story.WithScenario("PendingScenario")
                    .Pending("Im not done yet")
                    .Given("something pending")
                    .When("some pending event occurs")
                    .Then("this text should still show up in xml output");
            }

            [Story]
            public void Story2()
            {
                Story story = new Story("S2");

                story
                    .AsA("X2")
                    .IWant("Y2")
                    .SoThat("Z2");

                story.WithScenario("SC1")
                    .Given("something", () => { })
                    .When("some event occurs", () => { })
                    .Then("there is some outcome", () => { });
            }
        }

        [Theme("T2")]
        public class ThemeClass2
        {
            [Story]
            public void Story1()
            {
                Story story = new Story("S3");

                story
                    .AsA("X3")
                    .IWant("Y3")
                    .SoThat("Z3");

                story.WithScenario("SC3")
                    .Given("something", () => { })
                    .When("some event occurs", () => { })
                    .Then("there is some outcome", () => { });
            }
        }
    }
}
