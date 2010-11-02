using System;

namespace NBehave.Narrator.Framework
{
    public class EventArgs<T> : EventArgs
    {
        private readonly T _eventData;

        public EventArgs(T eventData)
        {
            _eventData = eventData;
        }

        public T EventData
        {
            get { return _eventData; }
        }
    }
}