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
