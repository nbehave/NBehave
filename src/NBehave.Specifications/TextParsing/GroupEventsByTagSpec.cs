using System.Collections.Generic;
using System.Linq;
using NBehave.Domain;
using NBehave.TextParsing;
using NUnit.Framework;

namespace NBehave.Specifications.TextParsing
{
    [TestFixture]
    public class GroupEventsByTagSpec
    {
        [Test]
        public void Events_with_no_tags_returns_all_Events()
        {
            var feature = new Feature("title", "src");
            var expected = new List<GherkinEvent>
                               {
                                   new FeatureEvent(feature, e => { }),
                                   new ScenarioEvent(new Scenario("title", feature.Source, feature), e => { }),
                                   new StepEvent("step", e => { }),
                                   new EofEvent(e => { })
                               };
            var events = new Queue<GherkinEvent>(expected);
            var groupedEvents = GroupEventsByTag.GroupByTag(events);
            CollectionAssert.AreEqual(expected, groupedEvents);
        }

        [Test]
        public void events_with_tag_on_feature_set_tag_on_all_sub_events()
        {
            var feature = new Feature("title", "src");
            var events = new Queue<GherkinEvent>(new List<GherkinEvent>
                                                     {
                                                         new TagEvent("@tag", e => { }),
                                                         new FeatureEvent(feature, e => { }),
                                                         new ScenarioEvent(new Scenario("title", feature.Source, feature), e => { }),
                                                         new StepEvent("step", e => { }),
                                                         new EofEvent(e => { })
                                                     });
            var groupedEvents = GroupEventsByTag.GroupByTag(events);
            Assert.AreEqual(4, groupedEvents.Count);
            foreach (var @event in groupedEvents.Take(3))
                CollectionAssert.AreEqual(new[] {"@tag"}, @event.Tags);
            Assert.AreEqual(new string[0], groupedEvents.Last().Tags);
        }
    }
}