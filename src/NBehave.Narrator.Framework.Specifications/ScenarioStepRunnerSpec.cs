using System;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class ScenarioStepRunnerSpec
    {
        private IFeatureRunner _runner;
        private ActionCatalog _actionCatalog;
        private StringStepRunner _stringStepRunner;
        private ITinyMessengerHub _hub;

        private Scenario CreateScenario()
        {
            return new Scenario();
        }

        private void Init()
        {
            NBehaveInitialiser.Initialise(ConfigurationNoAppDomain.New.SetEventListener(Framework.EventListeners.EventListeners.NullEventListener()));
            _hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            _actionCatalog = TinyIoCContainer.Current.Resolve<ActionCatalog>();

            _stringStepRunner = new StringStepRunner(_actionCatalog, _hub);
            _runner = new FeatureRunner(_hub, _stringStepRunner);
        }

        private void RunScenarios(params Scenario[] scenarios)
        {
            var feature = new Feature("yadday yadda");
            foreach (var s in scenarios)
                feature.AddScenario(s);
            _runner.Run(feature);
        }

        [TestFixture]
        public class When_running_a_scenario : ScenarioStepRunnerSpec
        {
            private ScenarioResult _scenarioResult;
            private TinyMessageSubscriptionToken _subscription;

            [SetUp]
            public void SetUp()
            {
                Init();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);
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

                var scenario = CreateScenario();
                scenario.AddStep("Given my name is Axel");
                scenario.AddStep("And my name is Morgan");
                RunScenarios(scenario);
                Assert.AreEqual(2, _scenarioResult.StepResults.Count());
            }

            [Test]
            public void ShouldHaveDifferentResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenario();
                scenario.AddStep("Given my name is Morgan");
                scenario.AddStep("Given my name is Axel");
                RunScenarios(scenario);

                Assert.That(_scenarioResult.StepResults.First().Result, Is.TypeOf(typeof(Passed)));
                Assert.That(_scenarioResult.StepResults.Last().Result, Is.TypeOf(typeof(Failed)));
            }
        }

        [TestFixture]
        public class When_running_many_scenarios_and_class_with_ActionSteps_implements_NotificationMethodAttributes : ScenarioStepRunnerSpec
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
                Init();
                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count$"), action, action.Method, "Given", this));

                var firstScenario = CreateScenario();

                firstScenario.AddStep("Scenario: One");
                firstScenario.AddStep("Given something to count");
                var secondScenario = CreateScenario();
                secondScenario.AddStep("Scenario: Two");
                secondScenario.AddStep("Given something to count");
                secondScenario.AddStep("Given something to count");

                RunScenarios(firstScenario, secondScenario);
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

        [TestFixture]
        public class When_running_a_scenario_that_throws_exception_in_AfterScenario : ScenarioStepRunnerSpec
        {
            private ScenarioResult _scenarioResult;
            private TinyMessageSubscriptionToken _subscription;

            [Given(@"something")]
            public void GivenSomething()
            {
            }

            [AfterScenario]
            public void OnAfterScenario()
            {
                throw new ApplicationException("AfterScenario failed");
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                Init();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");

                RunScenarios(scenario);
            }

            [TearDown]
            public void Cleanup()
            {
                _hub.Unsubscribe<ScenarioResultEvent>(_subscription);
            }

            [Test]
            public void Should_set_scenario_as_failed()
            {
                Assert.That(_scenarioResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Should_not_fail_any_steps()
            {
                foreach (var stepResult in _scenarioResult.StepResults)
                {
                    Assert.That(stepResult, Is.Not.InstanceOf<Failed>());
                }
            }
        }

        [TestFixture]
        public class When_running_a_scenario_that_throws_exception_in_BeforeScenario : ScenarioStepRunnerSpec
        {
            private ScenarioResult _scenarioResult;
            private TinyMessageSubscriptionToken _subscription;

            [Given(@"something")]
            public void GivenSomething()
            {
            }

            [BeforeScenario]
            public void OnBeforeScenario()
            {
                throw new ApplicationException("OnBeforeScenario failed");
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                Init();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");

                RunScenarios(scenario);
            }

            [TearDown]
            public void Cleanup()
            {
                _hub.Unsubscribe<ScenarioResultEvent>(_subscription);
            }
            [Test]
            public void Should_set_scenario_as_failed()
            {
                Assert.That(_scenarioResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Should_not_fail_any_steps()
            {
                foreach (var stepResult in _scenarioResult.StepResults)
                {
                    Assert.That(stepResult, Is.Not.InstanceOf<Failed>());
                }
            }
        }

        [TestFixture]
        public class When_running_a_scenario_that_has_a_failing_given_step : ScenarioStepRunnerSpec
        {
            private TinyMessageSubscriptionToken _subscription;
            private ScenarioResult _scenarioResult;

            [SetUp]
            public void SetUp()
            {
                Init();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);

                Action given = () => { throw new NotImplementedException(); };
                Action when = () => { };
                Action then = () => { };

                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), given, given.Method, "Given", this));
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"this$"), when, when.Method, "When", this));
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"that happens$"), when, when.Method, "Then", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");
                scenario.AddStep("When this");
                scenario.AddStep("Then that happens");
                RunScenarios(scenario);
            }

            [TearDown]
            public void Cleanup()
            {
                _hub.Unsubscribe<ScenarioResultEvent>(_subscription);
            }
            [Test]
            public void Should_mark_all_following_steps_as_pending()
            {
                Assert.That(_scenarioResult.StepResults.ElementAt(0).Result, Is.InstanceOf<Failed>());
                Assert.That(_scenarioResult.StepResults.ElementAt(1).Result, Is.InstanceOf<Skipped>());
                Assert.That(_scenarioResult.StepResults.ElementAt(2).Result, Is.InstanceOf<Skipped>());
            }
        }


        [TestFixture]
        public class When_running_a_scenario_with_implemented_steps_and_one_step_calls_pend : ScenarioStepRunnerSpec
        {
            private ScenarioResult _scenarioResult;
            private TinyMessageSubscriptionToken _subscription;

            [Given(@"something")]
            public void GivenSomething()
            {
            }

            [Given(@"pend me")]
            public void PendThisStep()
            {
                Step.Pend("some reason");
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                Init();
                _subscription = _hub.Subscribe<ScenarioResultEvent>(_ => _scenarioResult = _.Content);

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));
                Action pendAction = PendThisStep;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"pend me$"), pendAction, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");
                scenario.AddStep("And pend me");

                RunScenarios(scenario);
            }

            [TearDown]
            public void Cleanup()
            {
                _hub.Unsubscribe<ScenarioResultEvent>(_subscription);
            }

            [Test]
            public void Should_set_scenario_as_Pending()
            {
                Assert.That(_scenarioResult.Result, Is.InstanceOf<Pending>());
            }

            [Test]
            public void Should_set_step_pend_me_as_pending()
            {
                Assert.AreEqual(1, _scenarioResult.StepResults.Count(_ => _.StringStep.StepResult.Result is Pending));
                var stepResult = _scenarioResult.StepResults.FirstOrDefault(_ => _.StringStep.StepResult.Result is Pending);
                Assert.That("And pend me", Is.EqualTo(stepResult.StringStep.Step));
            }
        }
    }
}