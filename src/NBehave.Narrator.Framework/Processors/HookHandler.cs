using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class HookHandler : IMessageProcessor, IDisposable
    {
        private bool disposed;
        private readonly ITinyMessengerHub hub;
        private readonly HooksCatalog hooks;
        private readonly List<KeyValuePair<TinyMessageSubscriptionToken, Type>> hubSubscriberTokens = new List<KeyValuePair<TinyMessageSubscriptionToken, Type>>();

        public HookHandler(ITinyMessengerHub hub, HooksCatalog hooks)
        {
            this.hub = hub;
            this.hooks = hooks;
            SubscribeToHubEvents();
        }

        private void SubscribeToHubEvents()
        {
            Subscribe<RunStartedEvent>(e => OnEvent<Hooks.BeforeRunAttribute>());
            Subscribe<RunFinishedEvent>(e => OnEvent<Hooks.AfterRunAttribute>());
            Subscribe<FeatureStartedEvent>(e => OnEvent<Hooks.BeforeFeatureAttribute>());
            Subscribe<FeatureFinishedEvent>(e => OnEvent<Hooks.AfterFeatureAttribute>());
            Subscribe<ScenarioStartedEvent>(e => OnEvent<Hooks.BeforeScenarioAttribute>());
            Subscribe<ScenarioFinishedEvent>(e => OnEvent<Hooks.AfterScenarioAttribute>());
            Subscribe<StepStartedEvent>(e => OnEvent<Hooks.BeforeStepAttribute>());
            Subscribe<StepFinishedEvent>(e => OnEvent<Hooks.AfterStepAttribute>());
        }

        private void OnEvent<T>()
        {
            foreach (var hook in hooks.OfType<T>())
            {
                try
                {
                    hook.Invoke();
                }
                catch (Exception)
                {
                    // hm...
                    throw;
                }       
            }
        }

        private void Subscribe<T>(Action<T> eventReceiver) where T : class, ITinyMessage
        {
            var token = hub.Subscribe(eventReceiver, true);
            hubSubscriberTokens.Add(new KeyValuePair<TinyMessageSubscriptionToken, Type>(token, typeof(T)));
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    disposed = true;
                    Unsubscribe();
                }
            }
        }

        private void Unsubscribe()
        {
            foreach (var tokenPair in hubSubscriberTokens)
            {
                var token = tokenPair.Key;
                var type = tokenPair.Value;
                hub.Unsubscribe(token, type);
            }
        }
    }
}