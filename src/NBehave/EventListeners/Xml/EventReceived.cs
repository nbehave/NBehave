// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventReceived.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventReceived type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    using System;

    public class EventReceived
    {
        public EventReceived(string message, EventType eventType)
        {
            Message = message;
            EventType = eventType;
            Time = DateTime.Now;
        }

        public EventType EventType { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : {1}", EventType, Message);
        }
    }
}