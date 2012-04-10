using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework.Processors
{
    public class HooksHandler : IDisposable
    {
        private bool disposed;
        private readonly HooksCatalog hooks;
        private IRunContextEvents contextEvents;

        public HooksHandler(HooksCatalog hooks)
        {
            this.hooks = hooks;
        }

        public void SubscribeToHubEvents(IRunContextEvents context)
        {
            contextEvents = context;
            context.OnRunStarted += OnRunStarted;
            context.OnRunFinished += OnRunFinished;
            context.OnFeatureStarted += OnFeatureStarted;
            context.OnFeatureFinished += OnFeatureFinished;
            context.OnScenarioStarted += OnScenarioStarted;
            context.OnScenarioFinished += OnScenarioFinished;
            context.OnStepStarted += OnStepStarted;
            context.OnStepFinished += OnStepFinished;
        }

        private void OnRunStarted(object sender, EventArgs e)
        {
            OnEvent<Hooks.BeforeRunAttribute>();
        }

        private void OnRunFinished(object sender, EventArgs<FeatureResults> e)
        {
            OnEvent<Hooks.AfterRunAttribute>();
        }

        private void OnFeatureStarted(object sender, EventArgs<Feature> e)
        {
            OnEvent<Hooks.BeforeFeatureAttribute>();
        }

        private void OnFeatureFinished(object sender, EventArgs<FeatureResult> e)
        {
            OnEvent<Hooks.AfterFeatureAttribute>();
        }

        private void OnScenarioStarted(object sender, EventArgs<Scenario> e)
        {
            OnEvent<Hooks.BeforeScenarioAttribute>();
        }

        private void OnScenarioFinished(object sender, EventArgs<ScenarioResult> e)
        {
            OnEvent<Hooks.AfterScenarioAttribute>();
        }

        private void OnStepStarted(object sender, EventArgs<StringStep> e)
        {
            OnEvent<Hooks.BeforeStepAttribute>();
        }

        private void OnStepFinished(object sender, EventArgs<StepResult> e)
        {
            OnEvent<Hooks.AfterStepAttribute>();
        }

        private void OnEvent<T>()
        {
            var tags = GetTags();
            foreach (var hook in hooks.OfType<T>())
            {
                try
                {
                    var hookAttribute = hook.HookAttrib;
                    if (hookAttribute.DontRunIfHasTags.Any() && hookAttribute.DontRunIfHasTags.Intersect(tags).Any())
                        return;
                    if (hookAttribute.RunIfHasTags.Any() && hookAttribute.RunIfHasTags.Intersect(tags).Any())
                        hook.Invoke();
                    if (hookAttribute.RunIfHasTags.Any() == false)
                        hook.Invoke();
                }
                catch (Exception)
                {
                    // hm...
                    throw;
                }
            }
        }

        private List<string> GetTags()
        {
            var tags = FeatureContext.Current.Tags.ToList();
            tags.AddRange(ScenarioContext.Current.Tags);
            return tags.Distinct().ToList();
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
            contextEvents.OnRunStarted -= OnRunStarted;
            contextEvents.OnRunFinished -= OnRunFinished;
            contextEvents.OnFeatureStarted -= OnFeatureStarted;
            contextEvents.OnFeatureFinished -= OnFeatureFinished;
            contextEvents.OnScenarioStarted -= OnScenarioStarted;
            contextEvents.OnScenarioFinished -= OnScenarioFinished;
            contextEvents.OnStepStarted -= OnStepStarted;
            contextEvents.OnStepFinished -= OnStepFinished;
        }
    }
}