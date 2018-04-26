using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Extensions;
using NBehave.Hooks;
using NBehave.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Specifications.Internal
{
    [TestFixture]
    public class FeatureRunnerSpec
    {
        private FeatureRunner featureRunner;
        private ActionCatalog actionCatalog;
        private HooksCatalog hooksCatalog;

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            hooksCatalog = new HooksCatalog();
            actionCatalog = new ActionCatalog();
            var runContext = new RunContext(MockRepository.GenerateStub<IContextHandler>(), new HooksHandler(hooksCatalog));
            IStringStepRunner stringStepRunner = new StringStepRunner(actionCatalog);
            featureRunner = new FeatureRunner(stringStepRunner, runContext);
        }

        private Feature FeatureWithScenario()
        {
            var feature = new Feature("FeatureTitle", "");
            var scenario = new Scenario("ScenarioTitle", "");
            feature.AddScenario(scenario);
            return feature;
        }

        private Feature FeatureWithOneScenarioWithOneStep()
        {
            Action action = () => { };
            actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));

            var feature = FeatureWithScenario();
            var step = new StringStep("Given", "something", "");
            feature.Scenarios[0].AddStep(step);
            return feature;
        }

        [TestFixture]
        public class When_running_steps : FeatureRunnerSpec
        {
            private FeatureResult featureResult;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                Action<string> action = name => Assert.AreEqual("Morgan", name);
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));

                var f = FeatureWithScenario();
                var scenario = f.Scenarios[0];
                scenario.AddStep(new StringStep("And", "my name is Morgan", ""));
                scenario.AddStep(new StringStep("Given", "my name is Axel", ""));
                featureResult = featureRunner.Run(f);
            }

            [Test]
            public void ShouldHaveResultForEachStep()
            {
                Assert.AreEqual(2, featureResult.ScenarioResults[0].StepResults.Count());
            }

            [Test]
            public void ShouldHaveDifferentResultForEachStep()
            {
                Assert.That(featureResult.ScenarioResults[0].StepResults.First().Result, Is.TypeOf(typeof(Passed)));
                Assert.That(featureResult.ScenarioResults[0].StepResults.Last().Result, Is.TypeOf(typeof(Failed)));
            }
        }

        [TestFixture]
        public class When_running_scenario_with_table_steps_that_fail : FeatureRunnerSpec
        {
            [Test]
            public void Should_fail_scenario()
            {
                var feature = new Feature("title");
                var scenario = CreateScenarioWithTable(feature);
                feature.AddScenario(scenario);
                Action<string> action = x => { };
                actionCatalog.Add(new ActionMethodInfo("name $x".AsRegex(), action, action.Method, "Given"));
                actionCatalog.Add(new ActionMethodInfo("greeted".AsRegex(), action, action.Method, "When"));
                actionCatalog.Add(new ActionMethodInfo("Hello $y".AsRegex(), action, action.Method, "Then"));
                var result = featureRunner.Run(feature);
                Assert.IsNotNull(result);
                var scenarioResult = result.ScenarioResults[0];
                Assert.IsTrue(scenarioResult.HasFailedSteps());
            }

            private Scenario CreateScenarioWithTable(Feature feature)
            {
                var scenario = new Scenario("title", "", feature);
                var givenStep = new StringTableStep("Given", "name [x]", "");
                givenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("x") }), new Dictionary<string, string> { { "x", "Nisse" } }));
                givenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("x") }), new Dictionary<string, string> { { "x", "Kalle" } }));
                scenario.AddStep(givenStep);
                scenario.AddStep(new StringStep("When", "greeted", ""));
                var thenStep = new StringTableStep("Then", "Hello [y]", "");
                thenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("y"), }), new Dictionary<string, string> { { "y", "Nisse" } }));
                thenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("y"), }), new Dictionary<string, string> { { "y", "K�lle" } }));
                scenario.AddStep(thenStep);
                return scenario;
            }
        }

        [TestFixture]
        public class When_running_scenario_with_Hooks : FeatureRunnerSpec
        {
            bool beforeScenarioWasCalled;
            bool afterScenarioWasCalled;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                var feature = FeatureWithOneScenarioWithOneStep();
                beforeScenarioWasCalled = false;
                afterScenarioWasCalled = false;
                hooksCatalog.Add(new DelegateHookMetaData(() => { beforeScenarioWasCalled = true; }, new BeforeScenarioAttribute()));
                hooksCatalog.Add(new DelegateHookMetaData(() => { afterScenarioWasCalled = true; }, new AfterScenarioAttribute()));
                featureRunner.Run(feature);
            }

            [Test]
            public void Running_a_scenario_calls_before_scenario()
            {
                Assert.That(beforeScenarioWasCalled);
            }

            [Test]
            public void Running_a_scenario_calls_after_scenario()
            {
                Assert.That(afterScenarioWasCalled);
            }
        }

        [TestFixture]
        public class When_running_many_scenarios_with_Hooks : FeatureRunnerSpec
        {
            private int timesBeforeScenarioWasCalled;
            private int timesBeforeStepWasCalled;
            private int timesAfterStepWasCalled;
            private int timesAfterScenarioWasCalled;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                Action action = () => { };
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count"), action, action.Method, "Given", this));
                hooksCatalog.Add(new DelegateHookMetaData(() => { timesBeforeScenarioWasCalled++; }, new BeforeScenarioAttribute()));
                hooksCatalog.Add(new DelegateHookMetaData(() => { timesBeforeStepWasCalled++; }, new BeforeStepAttribute()));
                hooksCatalog.Add(new DelegateHookMetaData(() => { timesAfterStepWasCalled++; }, new AfterStepAttribute()));
                hooksCatalog.Add(new DelegateHookMetaData(() => { timesAfterScenarioWasCalled++; }, new AfterScenarioAttribute()));

                var f = new Feature("title", "");
                var firstScenario = new Scenario();

                firstScenario.AddStep(new StringStep("Given", "something to count", ""));
                var secondScenario = new Scenario();
                secondScenario.AddStep(new StringStep("Given", "something to count", ""));
                secondScenario.AddStep(new StringStep("Given", "something to count", ""));
                f.AddScenario(firstScenario);
                f.AddScenario(secondScenario);
                featureRunner.Run(f);
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
        public class When_first_step_throws_exception : FeatureRunnerSpec
        {
            [Test]
            public void Should_Call_afterStep_if_step_fails()
            {
                bool afterStepWasCalled = false;
                Action action = () => { throw new NotImplementedException("fail"); };
                actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));
                hooksCatalog.Add(new DelegateHookMetaData(() => afterStepWasCalled = true, new AfterStepAttribute()));
                var feature = FeatureWithScenario();
                var step = new StringStep("Given", "something", "");
                feature.Scenarios[0].AddStep(step);
                featureRunner.Run(feature);
                Assert.That(afterStepWasCalled, Is.True);
            }
        }

        [TestFixture]
        public class When_BeforeStep_throws_exception : FeatureRunnerSpec
        {
            private StringStep step;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                var feature = FeatureWithOneScenarioWithOneStep();
                step = feature.Scenarios[0].Steps.First();
                hooksCatalog.Add(new DelegateHookMetaData(() => { throw new ArgumentException("BeforeStep"); }, new BeforeStepAttribute()));
                featureRunner.Run(feature);
            }

            [Test]
            public void should_fail_step()
            {
                Assert.That(step.StepResult.Result, Is.InstanceOf<Failed>());
                Assert.That(step.StepResult.Message, Does.Contain("ArgumentException"));
            }
        }

        [TestFixture]
        public class When_AfterStep_throws_exception : FeatureRunnerSpec
        {
            private StringStep step;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                var feature = FeatureWithOneScenarioWithOneStep();
                step = feature.Scenarios[0].Steps.First();
                hooksCatalog.Add(new DelegateHookMetaData(() => { throw new ArgumentNullException("AfterStep"); }, new AfterStepAttribute()));
                featureRunner.Run(feature);
            }

            [Test]
            public void Step_should_fail()
            {
                Assert.That(step.StepResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Step_Result_should_have_exception_message()
            {
                Assert.That(step.StepResult.Message, Does.Contain("ArgumentNullException"));
            }
        }

        [TestFixture]
        public class When_BeforeScenario_throws_exception : FeatureRunnerSpec
        {
            private bool afterStepWasCalled;
            private ScenarioResult scenarioResult;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                var feature = FeatureWithOneScenarioWithOneStep();
                hooksCatalog.Add(new DelegateHookMetaData(() => { throw new ApplicationException("OnBeforeScenario failed"); }, new BeforeScenarioAttribute()));
                hooksCatalog.Add(new DelegateHookMetaData(() => { afterStepWasCalled = true; }, new AfterScenarioAttribute()));

                var featureResult = featureRunner.Run(feature);
                scenarioResult = featureResult.ScenarioResults[0];
            }

            [Test]
            public void Should_set_scenario_as_failed()
            {
                Assert.That(scenarioResult.Result, Is.InstanceOf<Failed>());
            }

            [Test]
            public void Should_not_fail_any_steps()
            {
                Assert.IsFalse(scenarioResult.HasFailedSteps());
            }

            [Test]
            public void Should_call_AfterScenario()
            {
                Assert.That(afterStepWasCalled, Is.True);
            }
        }

        [TestFixture]
        public class When_AfterScenario_throws_exception : FeatureRunnerSpec
        {
            private ScenarioResult scenarioResult;

            [TestFixtureSetUp]
            public override void SetUp()
            {
                base.SetUp();
                var feature = FeatureWithOneScenarioWithOneStep();

                hooksCatalog.Add(new DelegateHookMetaData(() => { throw new ApplicationException("After"); }, new AfterScenarioAttribute()));
                var featureResult = featureRunner.Run(feature);
                scenarioResult = featureResult.ScenarioResults[0];
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
    }
}
