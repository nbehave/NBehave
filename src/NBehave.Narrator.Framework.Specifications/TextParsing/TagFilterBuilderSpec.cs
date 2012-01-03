using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.TextParsing;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

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
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title", "@tag1"), 
                                 new ScenarioEvent(hub, "title", "@tag1", "@tag2"), 
                                 new EofEvent()
                             };
            var filteredEvents = filter.Filter(events).ToList();
            CollectionAssert.AreEqual(events, filteredEvents);
        }

        [Test]
        public void tags_in_each_item_in_list_is_ORed_together()
        {
            var tagsToFilter = new List<string[]> { new[] { "@tag1", "@tag2" } };
            var filter = TagFilterBuilder.Build(tagsToFilter);
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new GherkinEvent[]
                             {
                                 new FeatureEvent(hub, "title"), 
                                 new ScenarioEvent(hub, "title", "@tag1"), 
                                 new ScenarioEvent(hub, "title", "@tag3"), 
                                 new ScenarioEvent(hub, "title", "@tag2"), 
                                 new ScenarioEvent(hub, "title", "@tag3"), 
                                 new EofEvent()
                             };
            var filteredEvents = filter.Filter(events).ToList();
            Assert.AreEqual(4, filteredEvents.Count);
            //no events with tag3
            Assert.IsFalse(filteredEvents.Any(_ => _.Tags.Any(t => t == "@tag3")));
        }
    }
}