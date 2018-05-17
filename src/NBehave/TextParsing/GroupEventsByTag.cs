using System.Collections.Generic;
using System.Linq;

namespace NBehave.TextParsing
{
    public class GroupEventsByTag
    {
        private readonly Queue<GherkinEvent> events;
        private readonly Queue<GherkinEvent> taggedEvents;
        private readonly List<string> featureTags = new List<string>();
        private readonly List<string> scenarioTags = new List<string>();
        private readonly List<string> tagQueue = new List<string>();

        private GroupEventsByTag(Queue<GherkinEvent> events)
        {
            this.events = events;
            taggedEvents = new Queue<GherkinEvent>(events.Count);
        }

        public static Queue<GherkinEvent> GroupByTag(Queue<GherkinEvent> events)
        {
            var m = new GroupEventsByTag(events);
            return m.GroupByTag();
        }

        private Queue<GherkinEvent> GroupByTag()
        {
            while (events.Any())
            {
                var e = events.Dequeue();
                if (e is TagEvent)
                    tagQueue.Add(e.Tags[0]);
                else
                {
                    HandleScenarioEvent(e);
                    HandleFeatureEvent(e);
                    HandleEofEvent(e);
                    AddTagsToEvent(e);
                }
            }
            return taggedEvents;
        }

        private void AddTagsToEvent(GherkinEvent e)
        {
            e.Tags.AddRange(featureTags);
            e.Tags.AddRange(scenarioTags);
            taggedEvents.Enqueue(e);
        }

        private void HandleEofEvent(GherkinEvent e)
        {
            if (e is EofEvent)
            {
                scenarioTags.Clear();
                featureTags.Clear();
                tagQueue.Clear();
            }
        }

        private void HandleFeatureEvent(GherkinEvent e)
        {
            if (e is FeatureEvent)
            {
                scenarioTags.Clear();
                featureTags.Clear();
                featureTags.AddRange(tagQueue);
                tagQueue.Clear();
            }
        }

        private void HandleScenarioEvent(GherkinEvent e)
        {
            if (e is ScenarioEvent)
            {
                scenarioTags.Clear();
                scenarioTags.AddRange(tagQueue);
                tagQueue.Clear();
            }
        }
    }
}