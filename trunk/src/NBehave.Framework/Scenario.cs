using System;
using System.Diagnostics;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public delegate void Action();

    public delegate void Action<TArg0, TArg1>(TArg0 arg0, TArg1 arg1);

    public delegate void Action<TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2);

    public delegate void Action<TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public class Scenario
    {
        private const string GivenType = "\t\tGiven";
        private readonly Story _story = null;
        private readonly string _title = null;
        private bool _isBlank;
        private bool _isPending;

        internal Scenario(string title, Story story)
        {
            Debug.Assert(story != null);

            _title = title;
            _story = story;
            _isBlank = true;
            _isPending = false;
        }

        public string Title
        {
            get { return _title; }
        }

        internal bool IsPending
        {
            get { return _isPending; }
        }

        internal Story Story
        {
            get { return _story; }
        }

        internal bool CanAddMessage
        {
            get { return !IsPending || _story.IsDryRun; }
        }

        private void AddBlankMessage()
        {
            if (!_isBlank)
            {
                AddFragmentMessage("");
            }

            _isBlank = false;
        }

        internal void AddFragmentMessage(string message)
        {
            if (CanAddMessage)
            {
                _story.AddMessage(message);
            }
        }

        public GivenFragment Given(string context, Action action)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeAction(GivenType, context, action);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeAction(GivenType, context, action, arg0);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeAction(GivenType, context, action, arg0, arg1);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeAction(GivenType, context, action, arg0, arg1, arg2);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeAction(GivenType, context, action, arg0, arg1, arg2, arg3);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given(string context)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeActionFromCatalog(GivenType, context);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeActionFromCatalog(GivenType, context, arg0);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2);
            }

            return new GivenFragment(this);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (CanAddMessage)
            {
                AddBlankMessage();

                _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2, arg3);
            }

            return new GivenFragment(this);
        }

        public Scenario Pending(string reason)
        {
            _story.AddMessage(string.Format("\t\tPending: {0}", reason));
            _story.PendLastScenarioResults(reason);

            _isPending = true;

            return this;
        }
    }
}