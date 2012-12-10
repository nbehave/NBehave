using System;

namespace NBehave.Narrator.Framework.Internal
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