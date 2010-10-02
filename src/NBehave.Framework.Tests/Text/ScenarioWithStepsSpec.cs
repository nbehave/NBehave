using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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

        protected abstract void EstablishContext();

        [TestFixtureSetUp]
        public void SetUp()
        {
            _calculatorSteps = new CalculatorSteps { Left = -1, Right = -1, Sum = -1 };
            _actionCatalog = new ActionCatalog();
            ParseAssemblyForSteps();
            _stringStepRunner = new StringStepRunner(_actionCatalog);
            EstablishContext();
        }

        private void ParseAssemblyForSteps()
        {
            var storyRunnerFilter = new StoryRunnerFilter(_calculatorSteps.GetType().Namespace, _calculatorSteps.GetType().Name, ".");
            var parser = new ActionStepParser(storyRunnerFilter, _actionCatalog);
            parser.FindActionSteps(_calculatorSteps.GetType().Assembly);
        }

        public class WhenRunningAScenario : ScenarioWithStepsSpec
        {
            private ScenarioResult _scenarioResult;
            private bool _scenarioCreatedCalled;

            protected override void EstablishContext()
            {
                ScenarioWithSteps.ScenarioCreated += (o, e) => { _scenarioCreatedCalled = true; };
                var scenarioWithSteps = new ScenarioWithSteps(_stringStepRunner) {Title = "scenario title"};
                scenarioWithSteps.AddStep("Given numbers 1 and 2");
                scenarioWithSteps.AddStep("When I add the numbers");
                scenarioWithSteps.AddStep("Then the sum is 3");

                _scenarioResult = scenarioWithSteps.Run();
            }

            [Test]
            public void ScenarioShouldPass()
            {
                Assert.That(_scenarioResult.Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void ScenarioShouldHave3PassingSteps()
            {
                var stepResults = _scenarioResult.ActionStepResults;

                Assert.That(stepResults.Count(), Is.EqualTo(3));
                foreach (var stepResult in stepResults)
                {
                    Assert.That(stepResult.Result, Is.TypeOf(typeof(Passed)));
                }
            }

            [Test]
            public void ShouldSetLeftOnCalculatorStep()
            {
                Assert.That(_calculatorSteps.Left, Is.EqualTo(1));
            }

            [Test]
            public void ShouldSetRightOnCalculatorStep()
            {
                Assert.That(_calculatorSteps.Right, Is.EqualTo(2));
            }

            [Test]
            public void ShouldSetSumOnCalculatorStep()
            {
                Assert.That(_calculatorSteps.Sum, Is.EqualTo(3));
            }

            [Test]
            public void ShouldRaiseScenarioCreatedEvent()
            {
                Assert.That(_scenarioCreatedCalled, Is.True, "Event was not called");
            }
        }

        public class WhenRunningAExampleScenario : ScenarioWithStepsSpec
        {
            private ScenarioResult _scenarioResult;

            protected override void EstablishContext()
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
            public void ShouldGetResultOfTypeScenarioExampleResult()
            {
                Assert.That(_scenarioResult, Is.TypeOf(typeof(ScenarioExampleResult)));
            }

            [Test]
            public void ShouldPassBothExamples()
            {
                Assert.That((_scenarioResult as ScenarioExampleResult).ExampleResults.Count(), Is.EqualTo(2));
                foreach (var scenarioResult in (_scenarioResult as ScenarioExampleResult).ExampleResults)
                    Assert.That(scenarioResult.Result, Is.TypeOf(typeof(Passed)));
            }
        }
    }
}