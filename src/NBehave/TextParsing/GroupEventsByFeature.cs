using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.TextParsing
{
    public static class GroupEventsByFeature
    {
        public static IEnumerable<GherkinEvent> GetEventsForNextFeature(Queue<GherkinEvent> events)
        {
            return GetEventsWhile(events, nextEvent => nextEvent is EofEvent || nextEvent is FeatureEvent);
        }

        private static IEnumerable<GherkinEvent> GetEventsWhile(Queue<GherkinEvent> events, Predicate<GherkinEvent> continueIfEventIsNot)
        {
            if (events.Any() == false)
                yield break;
            GherkinEvent nextEvent;
            do
            {
                yield return events.Dequeue();
                nextEvent = (events.Any()) ? events.Peek() : null;
            } while (events.Any() && !(continueIfEventIsNot(nextEvent)));
        }
    }
}