// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlOutputEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the XmlOutputEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    using System.Collections.Generic;
    using System.Xml;

    public class XmlOutputEventListener : EventListener
    {
        private readonly XmlOutputWriter _xmlOutputWriter;
        private readonly List<EventReceived> _eventsReceived = new List<EventReceived>();

        public XmlOutputEventListener(XmlWriter writer)
        {
            Writer = writer;
            _xmlOutputWriter = new XmlOutputWriter(Writer, _eventsReceived);
        }

        private XmlWriter Writer { get; set; }

        public override void RunStarted()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.RunStart));
        }

        public override void RunFinished()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.RunFinished));
            _xmlOutputWriter.WriteAllXml();
        }

        public override void ThemeStarted(string name)
        {
            _eventsReceived.Add(new EventReceived(name, EventType.ThemeStarted));
        }

        public override void ThemeFinished()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.ThemeFinished));
        }

        public override void FeatureStarted(string feature)
        {
            _eventsReceived.Add(new EventReceived(feature, EventType.FeatureCreated));
        }

        public override void FeatureNarrative(string message)
        {
            _eventsReceived.Add(new EventReceived(message, EventType.FeatureNarrative));
        }

        public override void ScenarioStarted(string scenario)
        {
            _eventsReceived.Add(new EventReceived(scenario, EventType.ScenarioCreated));
        }

        public override void ScenarioResult(ScenarioResult result)
        {
            _eventsReceived.Add(new ScenarioResultEventReceived(result));
        }
    }
}

