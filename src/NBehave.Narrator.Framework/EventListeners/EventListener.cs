// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Narrator.Framework
{
    public abstract class EventListener : MarshalByRefObject, IEventListener
    {
        public virtual void FeatureStarted(string feature)
        {
        }

        public virtual void FeatureNarrative(string message)
        {
        }

        public virtual void ScenarioStarted(string scenarioTitle)
        {
        }

        public virtual void RunStarted()
        {
        }

        public virtual void RunFinished()
        {
        }

        public virtual void ThemeStarted(string name)
        {
        }

        public virtual void ThemeFinished()
        {
        }

        public virtual void ScenarioResult(ScenarioResult result)
        {
        }
    }
}