using System.Collections.Generic;
using System.Linq;
using NBehave.Domain;
using NBehave.TextParsing;
using NUnit.Framework;

namespace NBehave.Specifications.TextParsing
{
    [TestFixture]
    public class TagFilterBuilderSpec
    {
        [Test]
        public void No_filter_should_not_remove_any_events()
        {
            var tagsToFilter = new List<string[]>();
            var filter = TagFilterBuilder.Build(tagsToFilter);
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(new Feature("title"), e => { }),
                                 new TagEvent("@tag1", e => { }), 
                                 new ScenarioEvent(new Scenario("title", ""), e => { }),
                                 new TagEvent("@tag1", e => { }), 
                                 new TagEvent("@tag2", e => { }),
                                 new ScenarioEvent(new Scenario("title", ""), e => { }),
                                 new EofEvent(e => { })
                             };
            var filteredEvents = filter.Filter(events).ToList();
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void tags_in_each_item_in_list_is_ORed_together()
        {
            var tagsToFilter = new List<string[]> { new[] { "@tag1", "@tag2" } };
            var filter = TagFilterBuilder.Build(tagsToFilter);
            var eventsInQueue = new Queue<GherkinEvent>();
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("title"), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag3", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag3", e => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), e => { }));
            eventsInQueue.Enqueue(new EofEvent(e => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            //no events with tag3
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag3")));
        }
    }
}