using System;
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
                throw new ArgumentNullException("AfterScenario");
            }

        }
    }
}
