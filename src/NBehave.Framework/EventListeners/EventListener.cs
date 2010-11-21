// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public abstract class EventListener : IMessengerHubAware
    {
        public virtual void FeatureCreated(string feature)
        {
        }

        public virtual void FeatureNarrative(string message)
        {
        }

        public virtual void ScenarioCreated(string scenarioTitle)
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

        public virtual void Initialise(ITinyMessengerHub hub)
        {
            hub.Subscribe<FeatureCreated>(created => this.FeatureCreated(created.Content));
            hub.Subscribe<FeatureNarrative>(narrative => this.FeatureNarrative(narrative.Content));
            hub.Subscribe<ScenarioCreated>(created => this.ScenarioCreated(created.Content));
            hub.Subscribe<RunStarted>(started => this.RunStarted());
            hub.Subscribe<RunFinished>(finished => this.RunFinished());
            hub.Subscribe<ThemeStarted>(themeStarted => this.ThemeStarted(themeStarted.Content));
            hub.Subscribe<ThemeFinished>(themeFinished => this.ThemeFinished());
            hub.Subscribe<ScenarioResultMessage>(message => this.ScenarioResult(message.Content));
        }
    }
}