using System;

namespace NBehave.Narrator.Framework
{
    public class ThenFragment : ScenarioFragment<ThenFragment>
    {
        internal ThenFragment(Scenario scenario) : base(scenario) { }

        public GivenFragment Given(string context, Action action)
        {
            return Scenario.Given(context, action);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            return Scenario.Given(context, arg0, action);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            return Scenario.Given(context, arg0, arg1, action);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
        {
            return Scenario.Given(context, arg0, arg1, arg2, action);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            return Scenario.Given(context, arg0, arg1, arg2, arg3, action);
        }

        public GivenFragment Given(string context)
        {
            return Scenario.Given(context);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0)
        {
            return Scenario.Given(context, arg0);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            return Scenario.Given(context, arg0, arg1);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            return Scenario.Given(context, arg0, arg1, arg2);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return Scenario.Given(context, arg0, arg1, arg2, arg3);
        }

        public Scenario WithScenario(string title)
        {
            return Scenario.Story.WithScenario(title);
        }
    }
}