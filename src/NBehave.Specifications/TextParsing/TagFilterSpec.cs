using System.Collections.Generic;
using System.Linq;
using NBehave.TextParsing;
using NBehave.TextParsing.TagFilter;
using NUnit.Framework;

namespace NBehave.Specifications.TextParsing
{
    [TestFixture]
    public class TagFilterSpec
    {
        [Test]
        public void Empty_filter_should_return_all_events()
        {
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(new Feature("title"), e => { }), 
                                 new ScenarioEvent(new Scenario("title", ""), e => { }), 
                                 new StepEvent("step", e => { }), 
                                 new EofEvent(e => { })
                             };
            var filter = new NoFilter();
            var filteredEvents = filter.Filter(events);
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void Should_filter_events_by_tag_with_or()
        {
            var eventsInQueue = new Queue<GherkinEvent>();
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("title"), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new EofEvent(e => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filter = new OrFilter(new[] { "@tag1" });
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag2")));
        }

        [Test]
        public void Should_filter_events_by_exclude_tag_with_or()
        {
            var eventsInQueue = new Queue<GherkinEvent>();
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("title"), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new EofEvent(e => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filter = new OrFilter(new[] { "~@tag1" });
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag1")));
        }

        [Test]
        public void Should_or_multiple_tags()
        {
            var eventsInQueue = new Queue<GherkinEvent>();
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("title"), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new EofEvent(e => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filter = new OrFilter(new[] { "@tag1", "@tag2" });
            var filteredEvents = filter.Filter(events).ToList();
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void Should_be_able_to_AND_two_filters()
        {
            var eventsInQueue = new Queue<GherkinEvent>();
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("feature title"), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title t1", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title t2", ""), e => { }));
            eventsInQueue.Enqueue(new StepEvent("step", e => { }));
            eventsInQueue.Enqueue(new EofEvent(e => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filter1 = new OrFilter(new[] { "@tag1" });
            var filter2 = new OrFilter(new[] { "@tag2" });
            var filter = new AndFilter(filter1, filter2);
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            var scenarioEvents = filteredEvents.Where(_ => _ is ScenarioEvent).ToList();
            Assert.AreEqual(1, scenarioEvents.Count());
            CollectionAssert.AreEqual(new[] { "@tag1", "@tag2" }, scenarioEvents[0].Tags);
        }
    }
}