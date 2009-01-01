using System;
using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
    public class ScenarioMessage
    {
        public ScenarioMessage(string category, string message)
        {
            Category = category;
            Message = message;
        }

        public string Category { get; private set; }
        public string Message { get; private set; }
    }

    public class Scenario
    {
        private const string GivenType = "Given";

        public event EventHandler<EventArgs<ScenarioMessage>> ScenarioMessageAdded;

        internal Scenario(string title, Story story)
        {
            Debug.Assert(story != null);
            OnScenarioMessageAdded(new ScenarioMessage("Scenario Title", title));

            Title = title;
            Story = story;
            IsPending = false;
        }

        public string Title { get; private set; }

        internal bool IsPending { get; private set; }

        internal Story Story { get; private set; }

        internal bool CanAddMessage
        {
            get { return !IsPending || Story.IsDryRun; }
        }

        protected void OnScenarioMessageAdded(ScenarioMessage scenarioMessageEventArgs)
        {
            if (ScenarioMessageAdded == null)
                return;

            var e = new EventArgs<ScenarioMessage>(scenarioMessageEventArgs);
            ScenarioMessageAdded(this, e);
            //Story.AddMessage(e.EventData);
        }

        public GivenFragment Given(string context, Action action)
        {
            if (CanAddMessage)
            {
                Story.InvokeAction(GivenType, context, action);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            if (CanAddMessage)
            {
                Story.InvokeAction(GivenType, context, action, arg0);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            if (CanAddMessage)
            {
                Story.InvokeAction(GivenType, context, action, arg0, arg1);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                        Action<TArg0, TArg1, TArg2> action)
        {
            if (CanAddMessage)
            {
                Story.InvokeAction(GivenType, context, action, arg0, arg1, arg2);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                               TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            if (CanAddMessage)
            {
                Story.InvokeAction(GivenType, context, action, arg0, arg1, arg2, arg3);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given(string context)
        {
            if (CanAddMessage)
            {
                Story.InvokeActionFromCatalog(GivenType, context);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0)
        {
            if (CanAddMessage)
            {
                Story.InvokeActionFromCatalog(GivenType, context, arg0);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            if (CanAddMessage)
            {
                Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (CanAddMessage)
            {
                Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2);
            }
            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                               TArg3 arg3)
        {
            if (CanAddMessage)
            {
                Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2, arg3);
            }
            return new GivenFragment(this);
        }

        public Scenario Pending(string reason)
        {
            if (Story.IsDryRun == false)
                OnScenarioMessageAdded(new ScenarioMessage("Pending", reason));
            Story.PendLastScenarioResults(reason);

            IsPending = true;

            return this;
        }
    }
}