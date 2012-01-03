using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.TextParsing
{
    [TestFixture]
    public class GroupEventsByTagSpec
    {
        [Test]
        public void Events_with_no_tags_returns_all_Events()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var expected = new List<GherkinEvent>
                                    {
                                        new FeatureEvent(hub, "title"), new ScenarioEvent(hub, "title"), new StepEvent(hub, "step"), new EofEvent()
                                    };
            var events = new Queue<GherkinEvent>(expected);
            var groupedEvents = GroupEventsByTag.GroupByTag(events);
            CollectionAssert.AreEqual(expected, groupedEvents);
        }

        [Test]
        public void events_with_tag_on_feature_set_tag_on_all_sub_events()
        {
            var hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            var events = new Queue<GherkinEvent>(new List<GherkinEvent>
                                                     {
                                                         new TagEvent(hub, "@tag"),
                                                         new FeatureEvent(hub, "title"),
                                                         new ScenarioEvent(hub, "title"),
                                                         new StepEvent(hub, "step"),
                                                         new EofEvent()
                                                     });
            var groupedEvents = GroupEventsByTag.GroupByTag(events);
            Assert.AreEqual(4, groupedEvents.Count);
            foreach (var @event in groupedEvents.Take(3))
                CollectionAssert.AreEqual(new[] { "@tag" }, @event.Tags);
            Assert.AreEqual(new string[0], groupedEvents.Last().Tags);
        }
    }
}