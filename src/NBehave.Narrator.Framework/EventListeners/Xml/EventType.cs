// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventType.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public enum EventType
    {
        RunStart, 
        RunFinished,
        ThemeStarted, 
        ThemeFinished,
        FeatureCreated, 
        FeatureNarrative,
        ScenarioCreated, 
        ScenarioResult
    }
}