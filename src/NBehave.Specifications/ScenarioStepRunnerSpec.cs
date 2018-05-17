using System;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Internal;
using TinyIoC;
using NUnit.Framework;

namespace NBehave.Specifications
{
    //TODO: Move to FeatureRunnerSpec
    [TestFixture]
    public abstract class ScenarioStepRunnerSpec
    {
        private IFeatureRunner runner;
        private ActionCatalog actionCatalog;
        private StringStepRunner stringStepRunner;
        private ScenarioResult scenarioResult;

        private void Init()
        {
            NBehaveInitializer.Initialize(ConfigurationNoAppDomain.New.SetEventListener(NBehave.EventListeners.EventListeners.NullEventListener()));
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

                var scenario = new Scenario();
                scenario.AddStep(new StringStep("Given", "something", ""));
                scenario.AddStep(new StringStep("When", "this", ""));
                scenario.AddStep(new StringStep("Then", "that happens", ""));
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

            [OneTimeSetUp]
            public void Setup()
            {
                Init();

                Action action = GivenSomething;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something$"), action, action.Method, "Given", this));
                Action pendAction = PendThisStep;
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"pend me$"), pendAction, action.Method, "Given", this));

                var scenario = new Scenario();
                scenario.AddStep(new StringStep("Given", "something", ""));
                scenario.AddStep(new StringStep("And", "pend me", ""));

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
                Assert.That(stepResult.StringStep.Step, Is.EqualTo("And pend me"));
            }
        }
    }
}
