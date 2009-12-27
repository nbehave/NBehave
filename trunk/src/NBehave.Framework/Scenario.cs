using System;
using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
	//Not all is obsolete, only the fluent interface parts
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class Scenario
	{
		public event EventHandler<EventArgs<ScenarioMessage>> ScenarioMessageAdded;

		internal Scenario(Feature story)
		{
			Debug.Assert(story != null);
			Feature = story;
			IsPending = false;
		}

        internal Scenario(string title, Feature story)
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

        internal Feature Feature { get; private set; }

		private void OnScenarioMessageAdded(ScenarioMessage scenarioMessageEventArgs)
		{
			if (ScenarioMessageAdded == null)
				return;

			var e = new EventArgs<ScenarioMessage>(scenarioMessageEventArgs);
			ScenarioMessageAdded(this, e);
		}

        //public Scenario Pending(string reason)
        //{
        //    if (Feature.IsDryRun == false)
        //        OnScenarioMessageAdded(new ScenarioMessage("Pending", reason));
        //    Feature.PendLastScenarioResults(reason);

        //    IsPending = true;

        //    return this;
        //}
	}
}
