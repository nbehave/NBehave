using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

		internal Scenario(Story story)
		{
			Debug.Assert(story != null);
			Story = story;
			IsPending = false;
		}

		internal Scenario(string title, Story story)
			:this(story)
		{
			Title = title;
		}

		private static readonly Regex _regex = new Regex(@"^\s*Scenario(:?)");
		public static bool IsScenarioTitle(string text)
		{
			return _regex.IsMatch(text);
		}

		public static string GetTitle(string text)
		{
			var match = _regex.Match(text);
			return text.Substring(match.Value.Length).TrimStart(new[] { ' ', '\t' });
		}

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnScenarioMessageAdded(new ScenarioMessage("Scenario Title", _title));
			}
		}

		internal bool IsPending { get; set; }

		internal Story Story { get; private set; }

		private void OnScenarioMessageAdded(ScenarioMessage scenarioMessageEventArgs)
		{
			if (ScenarioMessageAdded == null)
				return;

			var e = new EventArgs<ScenarioMessage>(scenarioMessageEventArgs);
			ScenarioMessageAdded(this, e);
		}

		public GivenFragment Given(string context, Action action)
		{
			Story.InvokeAction(GivenType, context, action);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
		{
			Story.InvokeAction(GivenType, context, action, arg0);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
		{
			Story.InvokeAction(GivenType, context, action, arg0, arg1);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
		                                                Action<TArg0, TArg1, TArg2> action)
		{
			Story.InvokeAction(GivenType, context, action, arg0, arg1, arg2);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
		                                                       TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
		{
			Story.InvokeAction(GivenType, context, action, arg0, arg1, arg2, arg3);
			return new GivenFragment(this);
		}

		public GivenFragment Given(string context)
		{
			Story.InvokeActionFromCatalog(GivenType, context);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0>(string context, TArg0 arg0)
		{
			Story.InvokeActionFromCatalog(GivenType, context, arg0);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
		{
			Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2);
			return new GivenFragment(this);
		}

		public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
		                                                       TArg3 arg3)
		{
			Story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2, arg3);
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