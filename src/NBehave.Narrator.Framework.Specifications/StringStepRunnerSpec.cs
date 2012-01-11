using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class StringStepRunnerSpec
    {
        private IStringStepRunner _runner;
        private ActionCatalog _actionCatalog;
        private ITinyMessengerHub _hub;

        public virtual void Setup()
        {
            _actionCatalog = new ActionCatalog();
            _hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            _runner = new StringStepRunner(_actionCatalog, _hub);
        }

        [TestFixture]
        public class WhenRunningPlainTextScenarios : StringStepRunnerSpec
        {
            [SetUp]
            public override void Setup()
            {
                base.Setup();
            }

            [Test]
            public void ShouldInvokeActionGivenATokenString()
            {
                var wasCalled = false;
                Action<string> action = name => { wasCalled = true; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));
                _runner.Run(new StringStep("Given my name is Morgan", ""));
                Assert.IsTrue(wasCalled, "Action was not called");
            }

            [Test]
            public void ShouldGetParameterValueForAction()
            {
                var actual = string.Empty;
                Action<string> action = name => { actual = name; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));
                _runner.Run(new StringStep("Given my name is Morgan", ""));
                Assert.That(actual, Is.EqualTo("Morgan"));
            }

            [Test]
            public void ShouldReturnPendingIfActionGivenInTokenStringDoesntExist()
            {
                var step = new StringStep("Given this doesnt exist", "");
                _runner.Run(step);
                Assert.That(step.StepResult.Result, Is.TypeOf(typeof(PendingNotImplemented)));
            }

            [Test]
            public void Should_raise_StepStartedEvent_before_invoking_Step()
            {
                Action<string> action = name => _hub.AssertWasCalled(_ => _.Publish<StepStartedEvent>(null), o => o.IgnoreArguments());
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));
                _runner.Run(new StringStep("Given my name is Morgan", ""));
                _hub.AssertWasCalled(_ => _.Publish<StepStartedEvent>(null), o => o.IgnoreArguments().Repeat.Once());
            }

            [Test]
            public void Should_raise_StepFinishedEvent_after_invoking_Step()
            {
                Action<string> action = name => _hub.AssertWasNotCalled(_ => _.Publish<StepFinishedEvent>(null), o => o.IgnoreArguments());
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method, "Given"));
                _runner.Run(new StringStep("Given my name is Morgan", ""));
                _hub.AssertWasCalled(_ => _.Publish<StepFinishedEvent>(null), o => o.IgnoreArguments().Repeat.Once());
            }

            public class UserClass
            {
                public int Age { get; set; }
                public string Name { get; set; }
            }

            [Test]
            public void Should_get_parameter_value_for_action_with_parameter_of_complex_type()
            {
                UserClass actual = null;
                Action<UserClass> action = _ => { actual = _; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+) and I'm (?<age>\d+) years old"), action, action.Method, "Given"));
                _runner.Run(new StringStep("Given my name is Morgan and I'm 42 years old", ""));
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Name, Is.EqualTo("Morgan"));
                Assert.That(actual.Age, Is.EqualTo(42));
            }

            [Test]
            public void Should_get_parameter_value_for_action_with_parameter_of_List_of_complex_type()
            {
                List<UserClass> actual = null;
                Action<List<UserClass>> action = _ => { actual = _; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"some users:"), action, action.Method, "Given"));
                var tableStep = new StringTableStep("Given some users:", "");
                tableStep.AddTableStep(new Example(new ExampleColumns { new ExampleColumn("age"), new ExampleColumn("name") }, new Dictionary<string, string> { { "age", "42" }, { "name", "Morgan" } }));
                tableStep.AddTableStep(new Example(new ExampleColumns { new ExampleColumn("age"), new ExampleColumn("name") }, new Dictionary<string, string> { { "age", "666" }, { "name", "Lucifer" } }));
                _runner.Run(tableStep);
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Count, Is.EqualTo(2));
                Assert.That(actual[0].Name, Is.EqualTo("Morgan"));
                Assert.That(actual[0].Age, Is.EqualTo(42));
                Assert.That(actual[1].Name, Is.EqualTo("Lucifer"));
                Assert.That(actual[1].Age, Is.EqualTo(666));
            }
        }

        [TestFixture, ActionSteps]
        public class WhenClassWithActionStepsImplementsNotificationAttributes : StringStepRunnerSpec
        {
            private bool _beforeScenarioWasCalled;
            private bool _beforeStepWasCalled;
            private bool _afterStepWasCalled;
            private bool _afterScenarioWasCalled;

            [Given(@"something$")]
            public void GivenSomething()
            { }

            [BeforeScenario]
            public void OnBeforeScenario()
            {
                _beforeScenarioWasCalled = true;
            }

            [BeforeStep]
            public void OnBeforeStep()
            {
                _beforeStepWasCalled = true;
            }

            [AfterStep]
            public void OnAfterStep()
            {
                _afterStepWasCalled = true;
            }

            [AfterScenario]
            public void OnAfterScenario()
            {
                _afterScenarioWasCalled = true;
            }

            [SetUp]
            public override void Setup()
            {
                base.Setup();

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));

                _beforeScenarioWasCalled = false;
                _beforeStepWasCalled = false;
                _afterStepWasCalled = false;
                _afterScenarioWasCalled = false;
            }

            [Test]
            public void RunningAStepShouldCallMostAttributedMethods()
            {
                var actionStepText = new StringStep("something", "");
                _runner.Run(actionStepText);

                Assert.That(_beforeScenarioWasCalled);
                Assert.That(_beforeStepWasCalled);
                Assert.That(_afterStepWasCalled);
                Assert.That(!_afterScenarioWasCalled);
            }

            [Test]
            public void CompletingAScenarioShouldCallAllAttributedMethods()
            {
                var actionStepText = new StringStep("something", "");
                _runner.Run(actionStepText);
                _runner.OnCloseScenario();

                Assert.That(_beforeScenarioWasCalled);
                Assert.That(_beforeStepWasCalled);
                Assert.That(_afterStepWasCalled);
                Assert.That(_afterScenarioWasCalled);
            }
        }

        [TestFixture, ActionSteps]
        public class When_AfterStep_throws_exception : StringStepRunnerSpec
        {
            [SetUp]
            public override void Setup()
            {
                base.Setup();

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));
            }

            [Test]
            public void RunningAStepShouldCallMostAttributedMethods()
            {
                var step = new StringStep("something", "");
                _runner.Run(step);
                Assert.That(step.StepResult.Result, Is.InstanceOf<Failed>());
                Assert.That(step.StepResult.Message, Is.StringContaining("ArgumentNullException"));
            }

            [Given(@"When_AfterStep_throws_exception$")]
            public void GivenSomething()
            { }

            [AfterStep]
            public void AfterStep()
            {
                throw new ArgumentNullException("AfterStep");
            }

        }

        [TestFixture, ActionSteps]
        public class When_BeforeStep_throws_exception : StringStepRunnerSpec
        {
            private bool _afterStepWasCalled;

            [SetUp]
            public override void Setup()
            {
                base.Setup();

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));
            }

            [Test]
            public void RunningAStepShouldCallMostAttributedMethods()
            {
                var step = new StringStep("something", "");
                _runner.Run(step);
                Assert.That(step.StepResult.Result, Is.InstanceOf<Failed>());
                Assert.That(step.StepResult.Message, Is.StringContaining("ArgumentNullException"));
            }

            [Given(@"When_AfterStep_throws_exception$")]
            public void GivenSomething()
            { }

            [BeforeStep]
            public void BeforeStep()
            {
                throw new ArgumentNullException("BeforeStep");
            }

            [AfterStep]
            public void AfterStep()
            {
                _afterStepWasCalled = true;
            }

        }

        [TestFixture, ActionSteps]
        public class When_first_step_throws_exception : StringStepRunnerSpec
        {
            private bool _afterStepWasCalled;

            [SetUp]
            public override void Setup()
            {
                base.Setup();

                Action action = GivenSomething;
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, "Given", this));
            }

            [Test]
            public void Should_Call_afterStep_if_step_fails()
            {
                var step = new StringStep("something", "");
                _runner.Run(step);
                Assert.That(_afterStepWasCalled, Is.True);
            }

            [Given(@"something")]
            public void GivenSomething()
            {
                throw new NotImplementedException("fail");
            }

            [AfterStep]
            public void AfterStep()
            {
                _afterStepWasCalled = true;
            }
        }
    }
}
