using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;


namespace NBehave.Narrator.Framework.Specifications
{
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    [TestFixture]
    public class ScenarioStepRunnerSpec
    {
        private ScenarioExecutor _runner;
        private ActionCatalog _actionCatalog;
        private StringStepRunner _stringStepRunner;

        private ScenarioWithSteps CreateScenarioWithSteps()
        {
            return new ScenarioWithSteps(_stringStepRunner, Tiny.TinyIoCContainer.Current.Resolve<ITinyMessengerHub>());
        }

        [SetUp]
        public void SetUp()
        {
            _actionCatalog = new ActionCatalog();
            _stringStepRunner = new StringStepRunner(_actionCatalog);
            _runner = new ScenarioExecutor(Tiny.TinyIoCContainer.Current.Resolve<ITinyMessengerHub>());
        }

        public class WhenRunningAScenario : ScenarioStepRunnerSpec
        {
            [Test]
            public void ShouldHaveResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenarioWithSteps();
                scenario.AddStep("Given my name is Axel");
                scenario.AddStep("And my name is Morgan");
                var scenarioResult = _runner.Run(new[] { scenario }).First();

                Assert.AreEqual(2, scenarioResult.ActionStepResults.Count());
            }

            [Test]
            public void ShouldHaveDifferentResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenarioWithSteps();
                scenario.AddStep("Given my name is Morgan");
                scenario.AddStep("Given my name is Axel");
                var scenarioResult = _runner.Run(new[] { scenario }).First();


                Assert.That(scenarioResult.ActionStepResults.First().Result, Is.TypeOf(typeof(Passed)));
                Assert.That(scenarioResult.ActionStepResults.Last().Result, Is.TypeOf(typeof(Failed)));
            }
        }

        [ActionSteps, TestFixture]
        public class WhenRunningManyScenariosAndClassWithActionStepsImplementsNotificationAttributes : ScenarioStepRunnerSpec
        {
            private int _timesBeforeScenarioWasCalled;
            private int _timesBeforeStepWasCalled;
            private int _timesAfterStepWasCalled;
            private int _timesAfterScenarioWasCalled;

            [Given(@"something$")]
            public void GivenSomething()
            { }

            [BeforeScenario]
            public void OnBeforeScenario()
            {
                _timesBeforeScenarioWasCalled++;
            }

            [BeforeStep]
            public void OnBeforeStep()
            {
                _timesBeforeStepWasCalled++;
            }

            [AfterStep]
            public void OnAfterStep()
            {
                _timesAfterStepWasCalled++;
            }

            [AfterScenario]
            public void OnAfterScenario()
            {
                _timesAfterScenarioWasCalled++;
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                SetUp();
                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count$"), action, action.Method, "Given", this));

                var firstScenario = CreateScenarioWithSteps();

                firstScenario.AddStep("Scenario: One");
                firstScenario.AddStep("Given something to count");
                var secondScenario = CreateScenarioWithSteps();
                secondScenario.AddStep("Scenario: Two");
                secondScenario.AddStep("Given something to count");
                secondScenario.AddStep("Given something to count");

                _runner.Run(new List<ScenarioWithSteps> { firstScenario, secondScenario });
            }

            [Test]
            public void ShouldCallBeforeScenarioOncePerScenario()
            {
                Assert.That(_timesBeforeScenarioWasCalled, Is.EqualTo(2));
            }

            [Test]
            public void ShouldCallAfterScenarioOncePerScenario()
            {
                Assert.That(_timesAfterScenarioWasCalled, Is.EqualTo(2));
            }

            [Test]
            public void ShouldCallBeforeStepOncePerStep()
            {
                Assert.That(_timesBeforeStepWasCalled, Is.EqualTo(3));
            }

            [Test]
            public void ShouldCallAfterStepOncePerStep()
            {
                Assert.That(_timesAfterStepWasCalled, Is.EqualTo(3));
            }
        }
    }
}