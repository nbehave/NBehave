// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlOutputEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the XmlOutputEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class XmlOutputEventListener : EventListener
    {
        private readonly XmlOutputWriter xmlOutputWriter;
        private readonly List<EventReceived> eventsReceived = new List<EventReceived>();
        private string feature;

        public XmlOutputEventListener(XmlWriter writer)
        {
            Writer = writer;
            xmlOutputWriter = new XmlOutputWriter(Writer, eventsReceived);
        }

        private XmlWriter Writer { get; set; }

        public override void RunStarted()
        {
            eventsReceived.Add(new EventReceived(string.Empty, EventType.RunStart));
        }

        public override void RunFinished()
        {
            eventsReceived.Add(new EventReceived(string.Empty, EventType.RunFinished));
            xmlOutputWriter.WriteAllXml();
        }

        public override void FeatureStarted(Feature feature)
        {
            this.feature = feature.Title;
            eventsReceived.Add(new EventReceived(feature.Title, EventType.FeatureStart));
            eventsReceived.Add(new EventReceived(feature.Narrative, EventType.FeatureNarrative));
        }

        public override void FeatureFinished(FeatureResult result)
        {
            eventsReceived.Add(new EventReceived(feature, EventType.FeatureFinished));
        }

        public override void ScenarioStarted(string scenario)
        {
            eventsReceived.Add(new EventReceived(scenario, EventType.ScenarioStart));
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            eventsReceived.Add(new ScenarioResultEventReceived(result));
        }
    }
}

