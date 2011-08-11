using System;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class EventProcessor : IMessageProcessor
    {
        public EventProcessor(ITinyMessengerHub hub, NBehaveConfiguration configuration)
        {
            if (AppDomain.CurrentDomain.FriendlyName == RunnerFactory.AppDomainName)
                return;

            hub.Subscribe<FeatureStartedEvent>(created => configuration.EventListener.FeatureStarted(created.Content));
            hub.Subscribe<FeatureNarrativeEvent>(narrative => configuration.EventListener.FeatureNarrative(narrative.Content));
            hub.Subscribe<ScenarioStartedEvent>(created => configuration.EventListener.ScenarioStarted(created.Content.Title));
            hub.Subscribe<RunStartedEvent>(started => configuration.EventListener.RunStarted());
            hub.Subscribe<RunFinishedEvent>(finished => configuration.EventListener.RunFinished());
            hub.Subscribe<ThemeStartedEvent>(themeStarted => configuration.EventListener.ThemeStarted(themeStarted.Content));
            hub.Subscribe<ThemeFinishedEvent>(themeFinished => configuration.EventListener.ThemeFinished());
            hub.Subscribe<ScenarioResultEvent>(message => configuration.EventListener.ScenarioResult(message.Content));
        }
    }
}