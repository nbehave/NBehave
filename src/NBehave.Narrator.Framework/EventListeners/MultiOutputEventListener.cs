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

        public override void FeatureStarted(string feature)
        {
            Invoke(l => l.FeatureStarted(feature));
        }

        public override void FeatureNarrative(string message)
        {
            Invoke(l => l.FeatureNarrative(message));
        }

        public override void ScenarioStarted(string scenarioTitle)
        {
            Invoke(l => l.ScenarioStarted(scenarioTitle));
        }

        public override void RunStarted()
        {
            Invoke(l => l.RunStarted());
        }

        public override void RunFinished()
        {
            Invoke(l => l.RunFinished());
        }

        public override void FeatureFinished(FeatureResult result)
        {
            Invoke(l => l.FeatureFinished(result));
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            Invoke(l => l.ScenarioFinished(result));
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
