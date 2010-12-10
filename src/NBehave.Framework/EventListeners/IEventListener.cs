// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the IEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public interface IEventListener
    {
        void FeatureCreated(string feature);

        void FeatureNarrative(string message);

        void ScenarioCreated(string scenarioTitle);

        void RunStarted();

        void RunFinished();

        void ThemeStarted(string name);

        void ThemeFinished();

        void ScenarioResult(ScenarioResult result);
    }
}