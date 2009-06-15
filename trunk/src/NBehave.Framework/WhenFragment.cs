using System;

namespace NBehave.Narrator.Framework
{
	public class WhenFragment : ScenarioFragment<WhenFragment>
	{
		private const string ThenType = "Then";

		internal WhenFragment(Scenario scenario, ScenarioBuilder scenarioBuilder)
			: base(scenario, scenarioBuilder) { }

		private ThenFragment NewThenFragment()
		{
			return new ThenFragment(Scenario, ScenarioBuilder);

		}
		
		public ThenFragment Then(string context, Action action)
		{
			Scenario.Story.InvokeAction(ThenType, context, action);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
		{
			Scenario.Story.InvokeAction(ThenType, context, action, arg0);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
		{
			Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
		{
			Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1, arg2);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
		{
			Scenario.Story.InvokeAction(ThenType, context, action, arg0, arg1, arg2, arg3);
			return NewThenFragment();
		}

		public ThenFragment Then(string context)
		{
			Scenario.Story.InvokeActionFromCatalog(ThenType, context);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0>(string context, TArg0 arg0)
		{
			Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
		{
			Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1, arg2);
			return NewThenFragment();
		}

		public ThenFragment Then<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			Scenario.Story.InvokeActionFromCatalog(ThenType, context, arg0, arg1, arg2, arg3);
			return NewThenFragment();
		}
	}
}