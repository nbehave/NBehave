using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace NBehave.Fluent.Framework.Specifications
{
    [TestFixture]
    public class When_using_step_runner_for_scenario_driven_tests
    {
        [Test]
        public void Should_allow_specified_step_implementations()
        {
            var stepRunner = new ScenarioDrivenSpecStepRunner(null);

            var didRun = false;

            stepRunner.RegisterImplementation(ScenarioFragment.Given, "change my flag", () => didRun = true);

            stepRunner.Run(new StringStep("Given change my flag", null));

            Assert.IsTrue(didRun);
        }

        [Test]
        public void Should_pick_up_step_implementations_via_reflection_from_supplied_helper_object()
        {
            var helper = new ReflectionBasedScenarioHelper();
            var stepRunner = new ScenarioDrivenSpecStepRunner(helper);

            stepRunner.Run(new StringStep("When using reflection to obtain a step", null));

            Assert.IsTrue(helper.HasRun);
        }

        [Test]
        public void Should_pick_up_step_implementations_via_attributes_from_supplied_helper_object()
        {
            var helper = new AttributedScenarioHelper();
            var stepRunner = new ScenarioDrivenSpecStepRunner(helper);

            stepRunner.Run(new StringStep("Given using attributes to obtain a step", null));

            Assert.That(helper.StepResult, Is.EqualTo(1));
        }

        [Test]
        public void Should_pick_up_step_implementations_via_attributes_from_supplied_helper_object_and_obtain_regex_captures()
        {
            var helper = new AttributedScenarioHelper();
            var stepRunner = new ScenarioDrivenSpecStepRunner(helper);

            stepRunner.Run(new StringStep("Given setting result via regex capture to 12", null));

            Assert.That(helper.StepResult, Is.EqualTo(12));
        }

        public class ReflectionBasedScenarioHelper
        {
            public bool HasRun { get; private set; }

            public void When_using_reflection_to_obtain_a_step()
            {
                HasRun = true;
            }
        }

        [ActionSteps]
        public class AttributedScenarioHelper
        {
            public int StepResult { get; private set; }

            [When("using attributes to obtain a step")]
            public void SetResultToOne()
            {
                StepResult = 1;
            }

            [When("setting result via regex capture to $result")]
            public void SetResultTo(int result)
            {
                StepResult = result;
            }
        }
    }
}
