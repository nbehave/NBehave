using System;

namespace NBehave.Narrator.Framework
{
	public class GivenFragment : ScenarioFragment<GivenFragment>
	{
		private const string WhenType = "When";

		internal GivenFragment(Scenario scenario, ScenarioBuilder scenarioBuilder) 
			: base(scenario, scenarioBuilder) { }

		private WhenFragment NewWhenFragment()
		{
			return new WhenFragment(Scenario, ScenarioBuilder);
		}
		
		public WhenFragment When(string context, Action action)
		{
			Scenario.Story.InvokeAction(WhenType, context, action);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
		{
			Scenario.Story.InvokeAction(WhenType, context, action, arg0);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
		{
			Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
		{
			Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1, arg2);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
		{
			Scenario.Story.InvokeAction(WhenType, context, action, arg0, arg1, arg2, arg3);
			return NewWhenFragment();
		}

		public WhenFragment When(string context)
		{
			Scenario.Story.InvokeActionFromCatalog(WhenType, context);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0>(string context, TArg0 arg0)
		{
			Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
		{
			Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1, arg2);
			return NewWhenFragment();
		}

		public WhenFragment When<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			Scenario.Story.InvokeActionFromCatalog(WhenType, context, arg0, arg1, arg2, arg3);
			return NewWhenFragment();
		}

	}
}