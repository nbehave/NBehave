using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioStepRunnerSpec
    {
        private ScenarioExecutor _runner;
        private ActionCatalog _actionCatalog;
        private StringStepRunner _stringStepRunner;

        private Scenario CreateScenarioWithSteps()
        {
            return new Scenario();
        }

        public class WhenRunningAScenario : ScenarioStepRunnerSpec
        {
            private ITinyMessengerHub _hub;
            private ScenarioResult _scenarioResult;
            private TinyMessageSubscriptionToken _subscription;

            [SetUp]
            public void SetUp()
            {
                _actionCatalog = new ActionCatalog();
                _stringStepRunner = new StringStepRunner(_actionCatalog);
                _hub = new TinyMessengerHub();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);
                _runner = new ScenarioExecutor(_hub, _stringStepRunner);
            }

            [TearDown]
            public void Cleanup()
            {
                _hub.Unsubscribe<ScenarioResultEvent>(_subscription);
            }

            [Test]
            public void ShouldHaveResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenarioWithSteps();
                scenario.AddStep("Given my name is Axel");
                scenario.AddStep("And my name is Morgan");
                _runner.Run(new[] {scenario});
                Assert.AreEqual(2, _scenarioResult.ActionStepResults.Count());
            }

            [Test]
            public void ShouldHaveDifferentResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenarioWithSteps();
                scenario.AddStep("Given my name is Morgan");
                scenario.AddStep("Given my name is Axel");
                _runner.Run(new[] {scenario});

                Assert.That(_scenarioResult.ActionStepResults.First().Result, Is.TypeOf(typeof (Passed)));
                Assert.That(_scenarioResult.ActionStepResults.Last().Result, Is.TypeOf(typeof (Failed)));
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
            {
            }

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
                NBehaveInitialiser.Initialise(TinyIoCContainer.Current, NBehaveConfiguration.New.SetEventListener(Framework.EventListeners.EventListeners.NullEventListener()));

                _actionCatalog = new ActionCatalog();
                _stringStepRunner = new StringStepRunner(_actionCatalog);
                _runner = new ScenarioExecutor(TinyIoCContainer.Current.Resolve<ITinyMessengerHub>(), _stringStepRunner);

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count$"), action, action.Method, "Given", this));

                var firstScenario = CreateScenarioWithSteps();

                firstScenario.AddStep("Scenario: One");
                firstScenario.AddStep("Given something to count");
                var secondScenario = CreateScenarioWithSteps();
                secondScenario.AddStep("Scenario: Two");
                secondScenario.AddStep("Given something to count");
                secondScenario.AddStep("Given something to count");

                _runner.Run(new List<Scenario> {firstScenario, secondScenario});
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