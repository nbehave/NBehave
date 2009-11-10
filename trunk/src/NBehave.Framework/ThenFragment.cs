using System;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class ThenFragment : ScenarioFragment<ThenFragment>
    {
        internal ThenFragment(Scenario scenario, ScenarioBuilder scenarioBuilder) 
        	: base(scenario, scenarioBuilder)
        {
        }

        public GivenFragment Given(string context, Action action)
        {
            return ScenarioBuilder.Given(context, action);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            return ScenarioBuilder.Given(context, arg0, action);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            return ScenarioBuilder.Given(context, arg0, arg1, action);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
        {
            return ScenarioBuilder.Given(context, arg0, arg1, arg2, action);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            return ScenarioBuilder.Given(context, arg0, arg1, arg2, arg3, action);
        }

        public GivenFragment Given(string context)
        {
            return ScenarioBuilder.Given(context);
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0)
        {
            return ScenarioBuilder.Given(context, arg0);
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            return ScenarioBuilder.Given(context, arg0, arg1);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            return ScenarioBuilder.Given(context, arg0, arg1, arg2);
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return ScenarioBuilder.Given(context, arg0, arg1, arg2, arg3);
        }

        public ScenarioBuilder WithScenario(string title)
        {
            return Scenario.Story.WithScenario(title);
        }
    }
}