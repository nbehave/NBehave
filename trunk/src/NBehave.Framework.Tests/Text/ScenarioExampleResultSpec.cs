using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ScenarioExampleResultSpec
    {
        private ScenarioExampleResult _scenarioExampleResult;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var steps = new[]
                            {
                                new StringStep("Given I have entered [num1] into the calculator", "", null),
                                new StringStep("And I have entered [num2] into the calculator", "", null),
                                new StringStep("When I add the numbers", "", null),
                                new StringStep("Then the sum should be [result]", "", null),
                            };
            _scenarioExampleResult = new ScenarioExampleResult(new Feature("featureTitle"), "scenarioTitle", steps, new List<Example>());
        }

        [Test]
        public void Should_have_original_steps()
        {
            var steps = _scenarioExampleResult.ActionStepResults.ToArray();
            Assert.That(steps[0].StringStep, Is.EqualTo("Given I have entered [num1] into the calculator"));
            Assert.That(steps[1].StringStep, Is.EqualTo("And I have entered [num2] into the calculator"));
            Assert.That(steps[2].StringStep, Is.EqualTo("When I add the numbers"));
            Assert.That(steps[3].StringStep, Is.EqualTo("Then the sum should be [result]"));
        }

        public class When_a_step_is_pending : ScenarioExampleResultSpec
        {
            [Test]
            public void Should_pend_correct_step()
            {
                var exampleResult = new ScenarioResult(new Feature("featureTitle"), "scenarioTitle");
                const string step = "Given I have entered [num1] into the calculator";
                exampleResult.AddActionStepResult(new ActionStepResult(step, new Pending("step is pending")));
                _scenarioExampleResult.AddResult(exampleResult);

                ActionStepResult actionStepResult = _scenarioExampleResult.ActionStepResults.First();
                Assert.That(actionStepResult.Result, Is.TypeOf(typeof(Pending)));
                StringAssert.StartsWith("step is pending", actionStepResult.Message);
            }
        }
    }
}