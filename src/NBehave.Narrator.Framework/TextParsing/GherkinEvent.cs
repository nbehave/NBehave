using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{

    public class FeatureEvent : GherkinEvent
    {
        public FeatureEvent(ITinyMessengerHub hub, string titleAndNarrative, params string[] tags)
            : base(() => hub.Publish(new ParsedFeature(hub, titleAndNarrative)), tags) { }
    }
    public class ScenarioEvent : GherkinEvent
    {
        public ScenarioEvent(ITinyMessengerHub hub, string scenarioTitle, params string[] tags)
            : base(() => hub.Publish(new ParsedScenario(hub, scenarioTitle)), tags) { }
    }
    public class ExamplesEvent : GherkinEvent
    {
        public ExamplesEvent(ITinyMessengerHub hub, params string[] tags)
            : base(() => hub.Publish(new EnteringExamples(hub)), tags) { }
    }
    public class StepEvent : GherkinEvent
    {
        public StepEvent(ITinyMessengerHub hub, string stepText, params string[] tags)
            : base(() => hub.Publish(new ParsedStep(hub, stepText)), tags) { }
    }
    public class TableEvent : GherkinEvent
    {
        public TableEvent(ITinyMessengerHub hub, IList<IList<Token>> columns, params string[] tags)
            : base(() => hub.Publish(new ParsedTable(hub, columns)), tags) { }
    }
    public class BackgroundEvent : GherkinEvent
    {
        public BackgroundEvent(ITinyMessengerHub hub, string backgroundTitle, params string[] tags)
            : base(() => hub.Publish(new ParsedBackground(hub, backgroundTitle)), tags) { }
    }
    public class TagEvent : GherkinEvent
    {
        public TagEvent(ITinyMessengerHub hub, string tag)
            : base(() => hub.Publish(new ParsedTag(hub, tag)), tag) { }
    }
    public class EofEvent : GherkinEvent
    {
        public EofEvent()
            : base(() => { }) { }
    }
    public abstract class GherkinEvent : IEquatable<GherkinEvent>
    {
        private readonly Action eventAction;

        protected GherkinEvent(Action eventAction, params string[] tags)
        {
            this.eventAction = eventAction;
            Tags = tags.ToList();
        }

        public List<string> Tags { get; private set; }

        public void RaiseEvent()
        {
            eventAction.Invoke();
        }

        public bool Equals(GherkinEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.eventAction, eventAction) && Equals(other.Tags, Tags);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(GherkinEvent)) return false;
            return Equals((GherkinEvent)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (eventAction.GetHashCode() * 397) ^ Tags.GetHashCode();
            }
        }

        public static bool operator ==(GherkinEvent left, GherkinEvent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GherkinEvent left, GherkinEvent right)
        {
            return !Equals(left, right);
        }
    }
}