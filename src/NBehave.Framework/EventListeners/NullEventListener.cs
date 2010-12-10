// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the NullEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    public class NullEventListener : IEventListener
    {
         void IEventListener.FeatureCreated(string feature)
        {
        }

        void IEventListener.FeatureNarrative(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {            
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.RunFinished()
        {
        }

        void IEventListener.ThemeStarted(string name)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult result)
        {            
        }
    }
}
