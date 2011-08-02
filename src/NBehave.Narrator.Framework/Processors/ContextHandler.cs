using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Processors
{
    public class ContextHandler : IMessageProcessor, IDisposable
    {
        private readonly ITinyMessengerHub _hub;
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;
        private readonly StepContext _stepContext;
        private bool _disposed;
        private readonly List<KeyValuePair<TinyMessageSubscriptionToken, Type>> _hubSubscriberTokens = new List<KeyValuePair<TinyMessageSubscriptionToken, Type>>();

        public ContextHandler(ITinyMessengerHub hub,
                              FeatureContext featureContext,
                              ScenarioContext scenarioContext,
                              StepContext stepContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _stepContext = stepContext;
            _hub = hub;
            SubscribeToHubEvents();
        }

        private void SubscribeToHubEvents()
        {
            Subscribe<FeatureStartedEvent>(OnFeatureCreated);
            Subscribe<FeatureFinishedEvent>(OnFeatureFinishedEvent);
            Subscribe<ScenarioStartedEvent>(OnScenarioCreatedEvent);
            Subscribe<ScenarioFinishedEvent>(OnScenarioFinishedEvent);
            Subscribe<StepStartedEvent>(OnStepCreatedEvent);
            Subscribe<StepFinishedEvent>(OnStepFinishedEvent);
        }

        private void Subscribe<T>(Action<T> eventReceiver) where T : class, ITinyMessage
        {
            var token = _hub.Subscribe(eventReceiver);
            _hubSubscriberTokens.Add(new KeyValuePair<TinyMessageSubscriptionToken, Type>(token, typeof(T)));
        }

        private void OnFeatureCreated(FeatureStartedEvent e)
        {
            _featureContext.FeatureTitle = e.Content;
        }

        private void OnFeatureFinishedEvent(FeatureFinishedEvent e)
        {
            DisposeContextValues(_featureContext);
        }

        private void OnScenarioCreatedEvent(ScenarioStartedEvent e)
        {
            _scenarioContext.ScenarioTitle = e.Content.Title;
        }

        private void OnScenarioFinishedEvent(ScenarioFinishedEvent e)
        {
            DisposeContextValues(_scenarioContext);
        }

        private void OnStepCreatedEvent(StepStartedEvent e)
        {
            _stepContext.Step = e.Content;
        }

        private void OnStepFinishedEvent(StepFinishedEvent e)
        {}

        private void DisposeContextValues(NBehaveContext context)
        {
            foreach (var value in context.Values)
            {
                var d = value as IDisposable;
                if (d != null)
                    d.Dispose();
            }
            context.Clear();
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
                if (!_disposed)
                {
                    _disposed = true;
                    Unsubscribe();
                }
            }
        }

        private void Unsubscribe()
        {
            foreach (var tokenPair in _hubSubscriberTokens)
            {
                var token = tokenPair.Key;
                var type = tokenPair.Value;
                _hub.Unsubscribe(token, type);
            }
        }
    }
}