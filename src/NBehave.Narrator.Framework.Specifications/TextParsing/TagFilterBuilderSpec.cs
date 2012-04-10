using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.TextParsing;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.TextParsing
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
                                 new FeatureEvent(new Feature("title"), () => { }),
                                 new TagEvent("@tag1", () => { }), 
                                 new ScenarioEvent(new Scenario("title", ""), () => { }),
                                 new TagEvent("@tag1", () => { }), 
                                 new TagEvent("@tag2", () => { }),
                                 new ScenarioEvent(new Scenario("title", ""), () => { }),
                                 new EofEvent(() => { })
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
            eventsInQueue.Enqueue(new FeatureEvent(new Feature("title"), () => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag1", () => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), () => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag3", () => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), () => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag2", () => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), () => { }));
            eventsInQueue.Enqueue(new TagEvent("@tag3", () => { }));
            eventsInQueue.Enqueue(new ScenarioEvent(new Scenario("title", ""), () => { }));
            eventsInQueue.Enqueue(new EofEvent(() => { }));
            var events = GroupEventsByTag.GroupByTag(eventsInQueue);
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            //no events with tag3
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag3")));
        }
    }
}