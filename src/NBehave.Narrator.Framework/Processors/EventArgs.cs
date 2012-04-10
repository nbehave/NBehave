using System;

namespace NBehave.Narrator.Framework.Processors
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