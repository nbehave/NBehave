using System;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class WhenFragment : ScenarioFragment<WhenFragment>
    {
        private const string ThenType = "\t\tThen";

        internal WhenFragment(Scenario scenario) : base(scenario) { }

        public ThenFragment Then(string context, Action action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(ThenType, context, action);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(ThenType, context, action, arg0);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1, arg2);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1, arg2, arg3);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then(string context)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(ThenType, context);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0>(string context, TArg0 arg0)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1, arg2);
            return new ThenFragment(Scenario);
        }

        public ThenFragment Then<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (Scenario.CanAddMessage)
                Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1, arg2, arg3);
            return new ThenFragment(Scenario);
        }
    }
}