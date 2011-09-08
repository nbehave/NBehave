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

            hub.Subscribe<RunStartedEvent>(_ => configuration.EventListener.RunStarted(), true);
            hub.Subscribe<RunFinishedEvent>(_ => configuration.EventListener.RunFinished(), true);
            hub.Subscribe<FeatureStartedEvent>(_ => configuration.EventListener.FeatureStarted(_.Content), true);
            hub.Subscribe<FeatureNarrativeEvent>(_ => configuration.EventListener.FeatureNarrative(_.Content), true);
            hub.Subscribe<FeatureResultEvent>(_ => configuration.EventListener.FeatureFinished(_.Content), true);
            hub.Subscribe<ScenarioStartedEvent>(_ => configuration.EventListener.ScenarioStarted(_.Content.Title), true);
            hub.Subscribe<ScenarioResultEvent>(_ => configuration.EventListener.ScenarioFinished(_.Content), true);
        }
    }
}