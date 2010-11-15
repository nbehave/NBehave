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

    public class XmlOutputEventListener : IEventListener
    {
        private readonly XmlOutputWriter _xmlOutputWriter;
        private readonly List<EventReceived> _eventsReceived = new List<EventReceived>();

        public XmlOutputEventListener(XmlWriter writer)
        {
            Writer = writer;
            _xmlOutputWriter = new XmlOutputWriter(Writer, _eventsReceived);
        }

        private XmlWriter Writer { get; set; }

        void IEventListener.RunStarted()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.RunStart));
        }

        void IEventListener.RunFinished()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.RunFinished));
            _xmlOutputWriter.WriteAllXml();
        }

        void IEventListener.ThemeStarted(string name)
        {
            _eventsReceived.Add(new EventReceived(name, EventType.ThemeStarted));
        }

        void IEventListener.ThemeFinished()
        {
            _eventsReceived.Add(new EventReceived(string.Empty, EventType.ThemeFinished));
        }

        void IEventListener.FeatureCreated(string feature)
        {
            _eventsReceived.Add(new EventReceived(feature, EventType.FeatureCreated));
        }

        void IEventListener.FeatureNarrative(string message)
        {
            _eventsReceived.Add(new EventReceived(message, EventType.FeatureNarrative));
        }

        void IEventListener.ScenarioCreated(string scenario)
        {
            _eventsReceived.Add(new EventReceived(scenario, EventType.ScenarioCreated));
        }

        void IEventListener.ScenarioResult(ScenarioResult result)
        {
            _eventsReceived.Add(new ScenarioResultEventReceived(result));
        }
    }
}

