using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.TextParsing.TagFilter;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.TextParsing
{
    [TestFixture]
    public class TagFilterSpec
    {
        [Test]
        public void Empty_filter_should_return_all_events()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title"), 
                                 new StepEvent(hub, "step"), 
                                 new EofEvent()
                             };
            var filter = new NoFilter();
            var filteredEvents = filter.Filter(events);
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void Should_filter_events_by_tag_with_or()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title", "@tag1"), 
                                 new StepEvent(hub, "step", "@tag1"), 
                                 new ScenarioEvent(hub, "title", "@tag2"), 
                                 new StepEvent(hub, "step", "@tag2"), 
                                 new EofEvent()
                             };
            var filter = new OrFilter(new[] { "@tag1" });
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag2")));
        }

        [Test]
        public void Should_filter_events_by_exclude_tag_with_or()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title", "@tag1"), 
                                 new StepEvent(hub, "step", "@tag1"), 
                                 new ScenarioEvent(hub, "title", "@tag2"), 
                                 new StepEvent(hub, "step", "@tag2"), 
                                 new EofEvent()
                             };
            var filter = new OrFilter(new[] { "~@tag1" });
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag1")));
        }

        [Test]
        public void Should_or_multiple_tags()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title", "@tag1"), 
                                 new StepEvent(hub, "step", "@tag1"), 
                                 new ScenarioEvent(hub, "title", "@tag2"), 
                                 new StepEvent(hub, "step", "@tag2"), 
                                 new EofEvent()
                             };
            var filter = new OrFilter(new[] { "@tag1", "@tag2" });
            var filteredEvents = filter.Filter(events).ToList();
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void Should_be_able_to_AND_two_filters()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "feature title"), 
                                 new ScenarioEvent(hub, "title t1", "@tag1"), 
                                 new ScenarioEvent(hub, "title t2", "@tag1", "@tag2"), 
                                 new EofEvent()
                             };
            var filter1 = new OrFilter(new[] { "@tag1" });
            var filter2 = new OrFilter(new[] { "@tag2" });
            var filter = new AndFilter(filter1, filter2);
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(3, filteredEvents.Count);
            var scenarioEvents = filteredEvents.Where(_ => _ is ScenarioEvent).ToList();
            Assert.AreEqual(1, scenarioEvents.Count());
            CollectionAssert.AreEqual(new[]{"@tag1", "@tag2"}, scenarioEvents[0].Tags);
        }
    }
}