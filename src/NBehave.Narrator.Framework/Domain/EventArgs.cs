// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventArgs.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;

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