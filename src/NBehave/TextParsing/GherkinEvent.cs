using System;
using System.Collections.Generic;
using NBehave.Internal.Gherkin;

namespace NBehave.TextParsing
{
    public class FeatureEvent : GherkinEvent
    {
        public Feature Feature { get; private set; }

        public FeatureEvent(Feature feature, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            Feature = feature;
        }

    }
    public class ScenarioEvent : GherkinEvent
    {
        public Scenario Scenario { get; set; }

        public ScenarioEvent(Scenario scenario, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            Scenario = scenario;
        }
    }

    public class ExamplesEvent : GherkinEvent
    {
        public ExamplesEvent(Action<GherkinEvent> invokeEvent)
            : base(invokeEvent) { }
    }

    public class DocStringEvent : GherkinEvent
    {
        public DocStringEvent(string docString, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            DocString = docString;
        }

        public string DocString { get; private set; }
    }

    public class StepEvent : GherkinEvent
    {
        public StepEvent(string stepText, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            Step = stepText;
        }

        public string Step { get; private set; }
    }


    public class TableEvent : GherkinEvent
    {
        public IList<IList<Token>> Columns { get; private set; }

        public TableEvent(IList<IList<Token>> columns, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            Columns = columns;
        }
    }

    public class BackgroundEvent : GherkinEvent
    {
        public Scenario Scenario { get; set; }

        public BackgroundEvent(Scenario scenario, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent)
        {
            Scenario = scenario;
        }
    }

    public class TagEvent : GherkinEvent
    {
        public TagEvent(string tag, Action<GherkinEvent> invokeEvent)
            : base(invokeEvent) { Tags.Add(tag); }
    }

    public class EofEvent : GherkinEvent
    {
        public EofEvent(Action<GherkinEvent> invokeEvent)
            : base(invokeEvent) { }
    }

    public abstract class GherkinEvent : EventArgs, IEquatable<GherkinEvent>
    {
        private readonly Action<GherkinEvent> eventAction;

        protected GherkinEvent(Action<GherkinEvent> eventAction)
        {
            this.eventAction = eventAction;
            Tags = new List<string>();
        }

        public List<string> Tags { get; private set; }

        public void RaiseEvent()
        {
            eventAction.Invoke(this);
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