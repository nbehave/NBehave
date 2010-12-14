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
            hub.Subscribe<FeatureCreatedEvent>(created => this.FeatureCreated(created.Content));
            hub.Subscribe<FeatureNarrativeEvent>(narrative => this.FeatureNarrative(narrative.Content));
            hub.Subscribe<ScenarioCreatedEvent>(created => this.ScenarioCreated(created.Content.Title));
            hub.Subscribe<RunStartedEvent>(started => this.RunStarted());
            hub.Subscribe<RunFinishedEvent>(finished => this.RunFinished());
            hub.Subscribe<ThemeStartedEvent>(themeStarted => this.ThemeStarted(themeStarted.Content));
            hub.Subscribe<ThemeFinishedEvent>(themeFinished => this.ThemeFinished());
            hub.Subscribe<ScenarioResultEvent>(message => this.ScenarioResult(message.Content));
        }
    }
}