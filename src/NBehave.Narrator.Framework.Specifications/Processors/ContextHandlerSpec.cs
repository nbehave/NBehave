using NBehave.Narrator.Framework.Processors;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Processors
{
    [TestFixture]
    public abstract class ContextHandlerSpec
    {
        private ContextHandler sut;
        FeatureContext featureContext;
        ScenarioContext scenarioContext;
        StepContext stepContext;

        [SetUp]
        public void Initialize()
        {
            featureContext = new FeatureContext(null);
            scenarioContext = new ScenarioContext(featureContext, null);
            stepContext = new StepContext(featureContext, scenarioContext);
            sut = new ContextHandler(featureContext, scenarioContext, stepContext);
        }

        [TestFixture]
        public class When_FeatureEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_featureContext_with_feature_title()
            {
                const string featureTitle = "feature title";
                var f = new Feature(featureTitle);
                sut.OnFeatureStartedEvent(f);
                Assert.That(featureContext.FeatureTitle, Is.EqualTo(featureTitle));
            }

            [Test]
            public void Should_remove_all_items_from_FeatureContext_when_FeatureFinishedEvent_is_raised()
            {
                featureContext["Foo"] = "hello";
                const string featureTitle = "feature title";
                sut.OnFeatureFinishedEvent();
                Assert.That(featureContext.Values.Count, Is.EqualTo(0));
            }

            [Test]
            public void Should_set_tags_for_feature_event()
            {
                sut.OnParsedTagEvent("@tag1");
                sut.OnParsedTagEvent("@tag2");
                sut.OnFeatureStartedEvent(new Feature("featureTitle"));
                CollectionAssert.AreEqual(new[] { "tag1", "tag2" }, featureContext.Tags);
            }

            [Test]
            public void Should_remove_tags_on_featureEnd()
            {
                sut.OnParsedTagEvent("@tag1");
                sut.OnFeatureStartedEvent(new Feature("featureTitle"));
                CollectionAssert.IsNotEmpty(featureContext.Tags);
                sut.OnFeatureFinishedEvent();
                CollectionAssert.IsNotEmpty(featureContext.Tags);
                sut.OnFeatureStartedEvent(new Feature("featureTitle 2"));
                CollectionAssert.IsEmpty(featureContext.Tags);
            }
        }

        [TestFixture]
        public class When_ScenarioEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_scenarioContext_with_info_from_ScenarioCreated()
            {
                const string scenarioTitle = "scenario title";
                var scenario = new Scenario(scenarioTitle, "", new Feature("ignored"));
                sut.OnScenarioStartedEvent(scenario);
                Assert.That(scenarioContext.ScenarioTitle, Is.EqualTo(scenarioTitle));
            }

            [Test]
            public void Should_update_scenarioContext_with_info_from_FeatureContext()
            {
                const string featureTitle = "feature title";
                sut.OnFeatureStartedEvent(new Feature(featureTitle));
                const string scenarioTitle = "scenario title";
                var scenario = new Scenario(scenarioTitle, "", new Feature("ignored"));
                sut.OnScenarioStartedEvent(scenario);
                Assert.That(scenarioContext.FeatureContext, Is.SameAs(featureContext));
            }

            [Test]
            public void Should_remove_all_items_from_scenarioContext_when_ScenarioFinishedEvent_is_raised()
            {
                scenarioContext["Foo"] = "Bar";
                const string scenarioTitle = "scenario title";
                var content = new Scenario(scenarioTitle, "", new Feature("ignored"));
                sut.OnScenarioFinishedEvent();
                Assert.That(scenarioContext.Values.Count, Is.EqualTo(0));
            }

            [Test]
            public void Should_set_tags_for_scenario_event()
            {
                sut.OnParsedTagEvent("@tag1");
                sut.OnFeatureStartedEvent(new Feature("featureTitle"));
                sut.OnParsedTagEvent("tag2");
                sut.OnScenarioStartedEvent(new Scenario("scenario title", "", new Feature("featureTitle")));
                CollectionAssert.AreEqual(new[] { "tag1" }, featureContext.Tags);
                CollectionAssert.AreEqual(new[] { "tag1", "tag2" }, scenarioContext.Tags);
            }

            [Test]
            public void Should_remove_tags_for_previous_scenario_when_next_scenario_is_raised()
            {
                sut.OnParsedTagEvent("@tag1");
                sut.OnFeatureStartedEvent(new Feature("featureTitle"));
                sut.OnParsedTagEvent("@tag2");
                sut.OnScenarioStartedEvent(new Scenario("scenario title", "", new Feature("featureTitle")));
                sut.OnScenarioFinishedEvent();
                sut.OnParsedTagEvent("@tag3");
                sut.OnScenarioStartedEvent(new Scenario("scenario title", "", new Feature("featureTitle")));
                CollectionAssert.AreEqual(new[] { "tag1", "tag3" }, scenarioContext.Tags);
            }
        }

        [TestFixture]
        public class When_StepEvents_are_received : ContextHandlerSpec
        {
            [Test]
            public void Should_update_stepContext_with_info_from_StepCreatedEvent()
            {
                var step = "Given something".AsStringStep("");
                sut.OnStepStartedEvent(step);
                Assert.That(stepContext.Step, Is.EqualTo(step.Step));
            }

            [Test]
            public void Should_update_scenarioContext_with_info_from_ScenarioContext()
            {
                const string featureTitle = "feature title";
                sut.OnFeatureStartedEvent(new Feature(featureTitle));
                const string scenarioTitle = "scenario title";
                var scenario = new Scenario(scenarioTitle, "", new Feature("ignored"));
                sut.OnScenarioStartedEvent(scenario);
                var step = "Given something".AsStringStep("");
                sut.OnStepStartedEvent(step);
                Assert.That(stepContext.ScenarioContext, Is.SameAs(scenarioContext));
            }
        }
    }
}
