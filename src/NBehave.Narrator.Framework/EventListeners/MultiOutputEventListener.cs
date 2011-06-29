// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiOutputEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the MultiOutputEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    using System;

    public class MultiOutputEventListener : EventListener
    {
        private readonly IEventListener[] _listeners;

        public MultiOutputEventListener(params IEventListener[] listeners)
        {
            _listeners = listeners;
        }

        public IEventListener[] Listeners
        {
            get { return _listeners; }
        }

        public override void FeatureCreated(string feature)
        {
            Invoke(l => l.FeatureCreated(feature));
        }

        public override void FeatureNarrative(string message)
        {
            Invoke(l => l.FeatureNarrative(message));
        }

        public override void ScenarioCreated(string scenarioTitle)
        {
            Invoke(l => l.ScenarioCreated(scenarioTitle));
        }

        public override void RunStarted()
        {
            Invoke(l => l.RunStarted());
        }

        public override void RunFinished()
        {
            Invoke(l => l.RunFinished());
        }

        public override void ThemeStarted(string name)
        {
            Invoke(l => l.ThemeStarted(name));
        }

        public override void ThemeFinished()
        {
            Invoke(l => l.ThemeFinished());
        }

        public override void ScenarioResult(ScenarioResult result)
        {
            Invoke(l => l.ScenarioResult(result));
        }

        private void Invoke(Action<IEventListener> f)
        {
            foreach (var listener in _listeners)
            {
                f(listener);
            }
        }
    }
}
