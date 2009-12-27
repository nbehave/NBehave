using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [ActionSteps]
    public class CalculatorSteps
    {
        private static int _left = -1;
        private static int _right = -1;
        private static int _sum = -1;


        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public int Sum
        {
            get { return _sum; }
            set { _sum = value; }
        }

        [Given("numbers $left and $right")]
        public void Numbers(int left, int right)
        {
            Left = left;
            Right = right;
        }

        [When("I add the numbers")]
        public void Add()
        {
            Sum = Left + Right;
        }

        [Then("the sum is $sum")]
        public void SumIs(int sum)
        {
            Assert.That(sum, Is.EqualTo(Sum));
        }
    }

    [TestFixture]
    public abstract class ScenarioWithStepsSpec
    {
        private CalculatorSteps _calculatorSteps;
        private ActionCatalog _actionCatalog;
        private StringStepRunner _stringStepRunner;

        protected abstract void Establish_context();

        [TestFixtureSetUp]
        public void SetUp()
        {
            _calculatorSteps = new CalculatorSteps { Left = -1, Right = -1, Sum = -1 };
            _actionCatalog = new ActionCatalog();
            ParseAssemblyForSteps();
            _stringStepRunner = new StringStepRunner(_actionCatalog);
            Establish_context();
        }

        private void ParseAssemblyForSteps()
        {
            var storyRunnerFilter = new StoryRunnerFilter(_calculatorSteps.GetType().Namespace, _calculatorSteps.GetType().Name, ".");
            var parser = new ActionStepParser(storyRunnerFilter, _actionCatalog);
            parser.FindActionSteps(_calculatorSteps.GetType().Assembly);
        }

        public class When_running_a_scenario : ScenarioWithStepsSpec
        {
            private ScenarioResult _scenarioResult;
            private bool _scenarioCreatedCalled;

            protected override void Establish_context()
            {
                ScenarioWithSteps.ScenarioCreated += (o, e) => { _scenarioCreatedCalled = true; };
                var scenarioWithSteps = new ScenarioWithSteps(_stringStepRunner);
                scenarioWithSteps.Title = "scenario title";
                scenarioWithSteps.AddStep("Given numbers 1 and 2");
                scenarioWithSteps.AddStep("When I add the numbers");
                scenarioWithSteps.AddStep("Then the sum is 3");

                _scenarioResult = scenarioWithSteps.Run().FirstOrDefault();
            }

            [Test]
            public void Scenario_should_pass()
            {
                Assert.That(_scenarioResult.Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void Scenario_should_have_3_passing_steps()
            {
                var stepResults = _scenarioResult.ActionStepResults;

                Assert.That(stepResults.Count(), Is.EqualTo(3));
                foreach (var stepResult in stepResults)
                {
                    Assert.That(stepResult.Result, Is.TypeOf(typeof(Passed)));
                }
            }

            [Test]
            public void Should_set_Left_on_calculatorStep()
            {
                Assert.That(_calculatorSteps.Left, Is.EqualTo(1));
            }

            [Test]
            public void Should_set_Right_on_calculatorStep()
            {
                Assert.That(_calculatorSteps.Right, Is.EqualTo(2));
            }

            [Test]
            public void Should_set_Sum_on_calculatorStep()
            {
                Assert.That(_calculatorSteps.Sum, Is.EqualTo(3));
            }

            [Test]
            public void Should_raise_scenario_created_event()
            {
                Assert.That(_scenarioCreatedCalled, Is.True, "Event was not called");
            }
        }

        public class When_running_a_example_scenario : ScenarioWithStepsSpec
        {
            private IEnumerable<ScenarioResult> _scenarioResult;

            protected override void Establish_context()
            {
                var scenarioWithSteps = new ScenarioWithSteps(_stringStepRunner);
                scenarioWithSteps.AddStep("Given numbers [left] and [right]");
                scenarioWithSteps.AddStep("When I add the numbers");
                scenarioWithSteps.AddStep("Then the sum is [sum]");

                var columnNames = new ExampleColumns { "left", "right", "sum" };
                var examples = new List<Example>
                                   {
                                       new Example(columnNames, new Dictionary<string, string> { {"left", "1"}, {"right", "2"}, {"sum", "3"} }),
                                       new Example(columnNames, new Dictionary<string, string> { {"left", "2"}, {"right", "3"}, {"sum", "5"} }),
                                   };
                scenarioWithSteps.AddExamples(examples);
                _scenarioResult = scenarioWithSteps.Run();
            }

            [Test]
            public void Should_pass_both_examples()
            {
                Assert.That(_scenarioResult.Count(), Is.EqualTo(2));
                foreach (var scenarioResult in _scenarioResult)
                    Assert.That(scenarioResult.Result, Is.TypeOf(typeof(Passed)));
            }
        }
    }
}