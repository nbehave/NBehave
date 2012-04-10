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
        private IFeatureRunner runner;
        private ActionCatalog actionCatalog;
        private StringStepRunner stringStepRunner;
        private ScenarioResult scenarioResult;

        private Scenario CreateScenario()
        {
            return new Scenario();
        }

        private void Init()
        {
            NBehaveInitialiser.Initialise(ConfigurationNoAppDomain.New.SetEventListener(Framework.EventListeners.EventListeners.NullEventListener()));
            actionCatalog = TinyIoCContainer.Current.Resolve<ActionCatalog>();
            stringStepRunner = new StringStepRunner(actionCatalog);
            runner = new FeatureRunner(stringStepRunner, TinyIoCContainer.Current.Resolve<IRunContext>());
        }

        private void RunScenarios(params Scenario[] scenarios)
        {
            var feature = new Feature("yadday yadda");
            foreach (var s in scenarios)
                feature.AddScenario(s);
            var featureResult = runner.Run(feature);
            scenarioResult = featureResult.ScenarioResults.FirstOrDefault();
        }

        [TestFixture]
        public class When_running_a_scenario : ScenarioStepRunnerSpec
        {

            [SetUp]
            public void SetUp()
            {
                Init();
            }

            [Test]
            public void ShouldHaveResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenario();
                scenario.AddStep("Given my name is Axel");
                scenario.AddStep("And my name is Morgan");
                RunScenarios(scenario);
                Assert.AreEqual(2, scenarioResult.StepResults.Count());
            }

            [Test]
            public void ShouldHaveDifferentResultForEachStep()
            {
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var scenario = CreateScenario();
                scenario.AddStep("Given my name is Morgan");
                scenario.AddStep("Given my name is Axel");
                RunScenarios(scenario);

                Assert.That(scenarioResult.StepResults.First().Result, Is.TypeOf(typeof(Passed)));
                Assert.That(scenarioResult.StepResults.Last().Result, Is.TypeOf(typeof(Failed)));
            }
        }

        [TestFixture]
        public class When_running_many_scenarios_and_class_with_ActionSteps_implements_NotificationMethodAttributes : ScenarioStepRunnerSpec
        {
            private int timesBeforeScenarioWasCalled;
            private int timesBeforeStepWasCalled;
            private int timesAfterStepWasCalled;
            private int timesAfterScenarioWasCalled;

            [Given(@"something$")]
            public void GivenSomething()
            {
            }

            [BeforeScenario]
            public void OnBeforeScenario()
            {
                timesBeforeScenarioWasCalled++;
            }

            [BeforeStep]
            public void OnBeforeStep()
            {
                timesBeforeStepWasCalled++;
            }

            [AfterStep]
            public void OnAfterStep()
            {
                timesAfterStepWasCalled++;
            }

            [AfterScenario]
            public void OnAfterScenario()
            {
                timesAfterScenarioWasCalled++;
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                Init();
                Action action = GivenSomething;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count$"), action, action.Method, "Given", this));

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
                Assert.That(timesBeforeScenarioWasCalled, Is.EqualTo(2));
            }

            [Test]
            public void ShouldCallAfterScenarioOncePerScenario()
            {
                Assert.That(timesAfterScenarioWasCalled, Is.EqualTo(2));
            }

            [Test]
            public void ShouldCallBeforeStepOncePerStep()
            {
                Assert.That(timesBeforeStepWasCalled, Is.EqualTo(3));
            }

            [Test]
            public void ShouldCallAfterStepOncePerStep()
            {
                Assert.That(timesAfterStepWasCalled, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class When_running_a_scenario_that_throws_exception_in_AfterScenario : ScenarioStepRunnerSpec
        {
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

                Action action = GivenSomething;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");

                RunScenarios(scenario);
            }

            [Test]
            public void Should_set_scenario_as_failed()
            {
                Assert.That(scenarioResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Should_not_fail_any_steps()
            {
                foreach (var stepResult in scenarioResult.StepResults)
                {
                    Assert.That(stepResult, Is.Not.InstanceOf<Failed>());
                }
            }
        }

        [TestFixture]
        public class When_running_a_scenario_that_throws_exception_in_BeforeScenario : ScenarioStepRunnerSpec
        {
            private bool afterStepWasCalled;

            [Given(@"something")]
            public void GivenSomething()
            {
            }

            [BeforeScenario]
            public void OnBeforeScenario()
            {
                throw new ApplicationException("OnBeforeScenario failed");
            }

            [AfterStep]
            public void AfterStep()
            {
                afterStepWasCalled = true;
            }
            [TestFixtureSetUp]
            public void Setup()
            {
                Init();

                Action action = GivenSomething;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");

                RunScenarios(scenario);
            }

            [Test]
            public void Should_set_scenario_as_failed()
            {
                Assert.That(scenarioResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Should_not_fail_any_steps()
            {
                foreach (var stepResult in scenarioResult.StepResults)
                {
                    Assert.That(stepResult, Is.Not.InstanceOf<Failed>());
                }
            }

            [Test]
            public void Should_call_AfterScenario()
            {
                Assert.That(afterStepWasCalled, Is.True);
            }
        }

        [TestFixture]
        public class When_running_a_scenario_that_has_a_failing_given_step : ScenarioStepRunnerSpec
        {
            [SetUp]
            public void SetUp()
            {
                Init();

                Action given = () => { throw new NotImplementedException(); };
                Action when = () => { };
                Action then = () => { };

                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), given, given.Method, "Given", this));
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"this$"), when, when.Method, "When", this));
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"that happens$"), when, when.Method, "Then", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");
                scenario.AddStep("When this");
                scenario.AddStep("Then that happens");
                RunScenarios(scenario);
            }

            [Test]
            public void Should_mark_all_following_steps_as_pending()
            {
                Assert.That(scenarioResult.StepResults.ElementAt(0).Result, Is.InstanceOf<Failed>());
                Assert.That(scenarioResult.StepResults.ElementAt(1).Result, Is.InstanceOf<Skipped>());
                Assert.That(scenarioResult.StepResults.ElementAt(2).Result, Is.InstanceOf<Skipped>());
            }
        }

        [TestFixture]
        public class When_running_a_scenario_with_implemented_steps_and_one_step_calls_pend : ScenarioStepRunnerSpec
        {
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

                Action action = GivenSomething;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));
                Action pendAction = PendThisStep;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"pend me$"), pendAction, action.Method, "Given", this));

                var scenario = CreateScenario();
                scenario.AddStep("Given something");
                scenario.AddStep("And pend me");

                RunScenarios(scenario);
            }

            [Test]
            public void Should_set_scenario_as_Pending()
            {
                Assert.That(scenarioResult.Result, Is.InstanceOf<Pending>());
            }

            [Test]
            public void Should_set_step_pend_me_as_pending()
            {
                Assert.AreEqual(1, scenarioResult.StepResults.Count(_ => _.StringStep.StepResult.Result is Pending));
                var stepResult = scenarioResult.StepResults.FirstOrDefault(_ => _.StringStep.StepResult.Result is Pending);
                Assert.That("And pend me", Is.EqualTo(stepResult.StringStep.Step));
            }
        }
    }
}