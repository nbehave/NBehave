using System;

namespace NBehave.Internal
{
    public class EventArgs<T> : EventArgs
    {
        public readonly T EventInfo;

        public EventArgs(T eventInfo)
        {
            EventInfo = eventInfo;
        }
    }
}