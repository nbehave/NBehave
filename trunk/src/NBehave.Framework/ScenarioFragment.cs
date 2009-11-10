using System;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
	public class ScenarioFragment<T>
		where T : ScenarioFragment<T>
	{
		private const string AndType = "And";
		private readonly Scenario _scenario;
		private readonly ScenarioBuilder _scenarioBuilder;

		protected ScenarioFragment(Scenario scenario, ScenarioBuilder scenarioBuilder)
		{
			if (scenario == null)
				throw new ArgumentNullException("scenario");
			_scenario = scenario;

			if (scenarioBuilder == null)
				throw new ArgumentNullException("scenarioBuilder");
			_scenarioBuilder = scenarioBuilder;
		}

		protected Scenario Scenario
		{
			get { return _scenario; }
		}

		protected ScenarioBuilder ScenarioBuilder
		{
			get { return _scenarioBuilder; }
		}

		public T And(string context, Action action)
		{
			Scenario.Story.InvokeAction(AndType, context, action);
			return (T)this;
		}

		public T And<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
		{
			Scenario.Story.InvokeAction(AndType, context, action, arg0);
			return (T)this;
		}

		public T And<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
		{
			Scenario.Story.InvokeAction(AndType, context, action, arg0, arg1);
			return (T)this;
		}

		public T And<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2> action)
		{
			Scenario.Story.InvokeAction(AndType, context, action, arg0, arg1, arg2);
			return (T)this;
		}

		public T And<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
		{
			Scenario.Story.InvokeAction(AndType, context, action, arg0, arg1, arg2, arg3);
			return (T)this;
		}

		public T And(string context)
		{
			Scenario.Story.InvokeActionFromCatalog(AndType, context);
			return (T)this;
		}

		public T And<TArg0>(string context, TArg0 arg0)
		{
			Scenario.Story.InvokeActionFromCatalog(AndType, context, arg0);
			return (T)this;
		}

		public T And<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
		{
			Scenario.Story.InvokeActionFromCatalog(AndType, context, arg0, arg1);
			return (T)this;
		}

		public T And<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			Scenario.Story.InvokeActionFromCatalog(AndType, context, arg0, arg1, arg2);
			return (T)this;
		}

		public T And<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			Scenario.Story.InvokeActionFromCatalog(AndType, context, arg0, arg1, arg2, arg3);
			return (T)this;
		}
	}
}