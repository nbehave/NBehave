using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class ScenarioResultEventArgs : EventArgs
    {
        private readonly Story _story;
        private readonly Scenario _scenario;

        public ScenarioResultEventArgs(Story story, Scenario scenario)
        {
            _story = story;
            _scenario = scenario;
        }

        public Story Story
        {
            get { return _story; }
        }

        public Scenario Scenario
        {
            get { return _scenario; }
        }

    }
}