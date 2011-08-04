using System;
using System.Linq;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.Processors
{
    [TestFixture]
    public abstract class ContextHandlerSpec
    {
        private ITinyMessengerHub _hub;
        private ContextHandler _sut;
        FeatureContext _featureContext;
        ScenarioContext _scenarioContext;
        StepContext _stepContext;

        [SetUp]
        public void Initialize()
        {
            _hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            _featureContext = new FeatureContext();
            _scenarioContext = new ScenarioContext(_featureContext);
            _stepContext = new StepContext(_featureContext, _scenarioContext);
            _sut = new ContextHandler(_hub, _featureContext, _scenarioContext, _stepContext);
        }

        [TearDown]
        public void Cleanup()
        {
            _sut.Dispose();
        }

        private void InvokeEvent<T>(T args) where T : class, ITinyMessage
        {
            var e = _hub.GetArgumentsForCallsMadeOn(_ => _.Subscribe<T>(null)).First();
            var evt = e[0] as Action<T>;
            evt.Invoke(args);
        }

        [TestFixture]
        public class When_FeatureEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_featureContext_with_feature_title()
            {
                const string featureTitle = "feature title";
                InvokeEvent(new FeatureStartedEvent(this, featureTitle));
                Assert.That(_featureContext.FeatureTitle, Is.EqualTo(featureTitle));
            }

            [Test]
            public void Should_remove_all_items_from_FeatureContext_when_FeatureFinishedEvent_is_raised()
            {
                _featureContext["Foo"] = "hello";
                const string featureTitle = "feature title";
                InvokeEvent(new FeatureFinishedEvent(this, featureTitle));
                Assert.That(_featureContext.Values.Count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class When_ScenarioEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_scenarioContext_with_info_from_ScenarioCreated()
            {
                const string scenarioTitle = "scenario title";
                var content = new Scenario(scenarioTitle, new Feature("ignored"));
                InvokeEvent(new ScenarioStartedEvent(this, content));
                Assert.That(_scenarioContext.ScenarioTitle, Is.EqualTo(scenarioTitle));
            }

            [Test]
            public void Should_update_scenarioContext_with_info_from_FeatureContext()
            {
                const string featureTitle = "feature title";
                InvokeEvent(new FeatureStartedEvent(this, featureTitle));
                const string scenarioTitle = "scenario title";
                var content = new Scenario(scenarioTitle, new Feature("ignored"));
                InvokeEvent(new ScenarioStartedEvent(this, content));
                Assert.That(_scenarioContext.FeatureContext, Is.SameAs(_featureContext));
            }

            [Test]
            public void Should_remove_all_items_from_scenarioContext_when_ScenarioFinishedEvent_is_raised()
            {
                _scenarioContext["Foo"] = "Bar";
                const string scenarioTitle = "scenario title";
                var content = new Scenario(scenarioTitle, new Feature("ignored"));
                InvokeEvent(new ScenarioFinishedEvent(this, content));
                Assert.That(_scenarioContext.Values.Count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class When_StepEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_stepContext_with_info_from_StepCreatedEvent()
            {
                const string step = "Given something";
                InvokeEvent(new StepStartedEvent(this, step));
                Assert.That(_stepContext.Step, Is.EqualTo(step));
            }

            [Test]
            public void Should_update_scenarioContext_with_info_from_ScenarioContext()
            {
                const string featureTitle = "feature title";
                InvokeEvent(new FeatureStartedEvent(this, featureTitle));
                const string scenarioTitle = "scenario title";
                var content = new Scenario(scenarioTitle, new Feature("ignored"));
                InvokeEvent(new ScenarioStartedEvent(this, content));
                const string step = "Given something";
                InvokeEvent(new StepStartedEvent(this, step));
                Assert.That(_stepContext.ScenarioContext, Is.SameAs(_scenarioContext));
            }
        }
    }
}
