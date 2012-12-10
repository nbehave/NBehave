using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework.TextParsing.TagFilter
{
    public abstract class TagFilter
    {
        protected readonly List<string> TheseTags;
        protected readonly List<string> NotTheseTags;

        protected TagFilter(string[] tags)
        {
            TheseTags = tags.Where(_ => _.StartsWith("~") == false).ToList();
            NotTheseTags = tags.Where(_ => _.StartsWith("~")).Select(_ => _.Substring(1)).ToList();
        }

        public IEnumerable<GherkinEvent> Filter(IEnumerable<GherkinEvent> events)
        {
            var scenarioEvents = GetScenariosToRun(events);
            return FilterEvents(scenarioEvents, events);
        }

        protected abstract IEnumerable<GherkinEvent> GetScenariosToRun(IEnumerable<GherkinEvent> events);

        protected IEnumerable<GherkinEvent> FilterEvents(IEnumerable<GherkinEvent> scenarioEventsToKeep, IEnumerable<GherkinEvent> allEvents)
        {
            var eventsToHandle = new Queue<GherkinEvent>(allEvents);
            if (scenarioEventsToKeep.Any())
            {
                while (eventsToHandle.Any() && eventsToHandle.Peek() is ScenarioEvent == false)
                    yield return eventsToHandle.Dequeue();
                while (eventsToHandle.Any())
                {
                    var e = eventsToHandle.Dequeue();
                    var eventsToRaise = DoEvent(e, scenarioEventsToKeep, eventsToHandle).ToList();
                    foreach (var @event in eventsToRaise)
                        yield return @event;
                }
            }
        }

        private IEnumerable<GherkinEvent> DoEvent(GherkinEvent e, IEnumerable<GherkinEvent> scenarioEvents, Queue<GherkinEvent> eventsToHandle)
        {
            if (e is ScenarioEvent)
            {
                var es = GetEventsForNextScenario(eventsToHandle);
                if (scenarioEvents.Contains(e))
                {
                    yield return e;
                    foreach (var ee in es)
                        yield return ee;
                }
            }
            else
                yield return e;
        }

        private IEnumerable<GherkinEvent> GetEventsForNextScenario(Queue<GherkinEvent> events)
        {
            return GetEventsWhile(events, nextEvent => nextEvent is ScenarioEvent || nextEvent is EofEvent || nextEvent is FeatureEvent).ToList();
        }

        private IEnumerable<GherkinEvent> GetEventsWhile(Queue<GherkinEvent> events, Predicate<GherkinEvent> continueIfEventIsNot)
        {
            if (events.Any() == false)
                yield break;
            GherkinEvent nextEvent = events.Peek();
            while (events.Any() && !(continueIfEventIsNot(nextEvent)))
            {
                yield return events.Dequeue();
                nextEvent = (events.Any()) ? events.Peek() : null;
            }
        }
    }

    public class NoFilter : TagFilter
    {
        public NoFilter()
            : base(new string[0])
        { }

        protected override IEnumerable<GherkinEvent> GetScenariosToRun(IEnumerable<GherkinEvent> events)
        {
            return events.ToList();
        }
    }

    public class OrFilter : TagFilter
    {
        public OrFilter(params string[] tags)
            : base(tags)
        { }

        protected override IEnumerable<GherkinEvent> GetScenariosToRun(IEnumerable<GherkinEvent> events)
        {

            var scenarioEvents = events.Where(_ => _ is ScenarioEvent).ToList();
            var includeOperand = scenarioEvents.Where(_ => TheseTags.Intersect(_.Tags).Any()).ToList();
            var includeOperand2 = NotTheseTags.Any() ? scenarioEvents.Except(scenarioEvents.Where(_ => NotTheseTags.Intersect(_.Tags).Any()).ToList()) : new GherkinEvent[0];
            var scenarioEventsToKeep = includeOperand.Union(includeOperand2).Distinct().ToList();
            return FilterEvents(scenarioEventsToKeep, events).ToList();
        }
    }

    public class AndFilter : TagFilter
    {
        private readonly List<TagFilter> filters;

        public AndFilter(params TagFilter[] filters)
            : base(new string[0])
        {
            this.filters = filters.ToList();
        }

        protected override IEnumerable<GherkinEvent> GetScenariosToRun(IEnumerable<GherkinEvent> events)
        {
            var scenarioEvents = events.Where(_ => _ is ScenarioEvent).ToList();
            var result = new List<GherkinEvent>(scenarioEvents);
            filters.ForEach(_ => result = _.Filter(result).ToList());
            return result.Distinct().ToList();
        }
    }
}