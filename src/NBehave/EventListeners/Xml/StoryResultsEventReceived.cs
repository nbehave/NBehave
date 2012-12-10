// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryResultsEventReceived.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioResultEventReceived type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Domain;

namespace NBehave.EventListeners.Xml
{
    public class ScenarioResultEventReceived : EventReceived
    {
        public ScenarioResultEventReceived(ScenarioResult results) 
            : base(string.Empty, EventType.ScenarioResult)
        {
            ScenarioResult = results;
        }

        public ScenarioResult ScenarioResult { get; private set; }
    }
}