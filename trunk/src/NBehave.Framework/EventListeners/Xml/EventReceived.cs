using System;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class EventReceived
    {
        public EventType EventType {get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
		
        public EventReceived(string message, EventType eventType)
        {
            Message = message;
            EventType = eventType;
            Time = DateTime.Now;
        }
		
    }
}