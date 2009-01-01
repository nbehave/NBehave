using System;

namespace NBehave.Narrator.Framework
{
    public class GivenFragment : ScenarioFragment<GivenFragment>
    {
        private const string WhenType = "When";

        internal GivenFragment(Scenario scenario) : base(scenario) { }

        public WhenFragment When(string context, Action action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(WhenType, context, action);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(WhenType, context, action, arg0);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1, arg2);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1, arg2, arg3);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When(string context)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(WhenType, context);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0>(string context, TArg0 arg0)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1, arg2);
            return new WhenFragment(Scenario);
        }

        public WhenFragment When<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1, arg2, arg3);
            return new WhenFragment(Scenario);
        }

    }
}