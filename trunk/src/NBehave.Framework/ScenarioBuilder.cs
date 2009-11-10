using System;

namespace NBehave.Narrator.Framework
{
	//Not all is obsolete, only the fluent interface parts
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class ScenarioBuilder
    {
        private const string GivenType = "Given";
        private Story _story;

        public Scenario Scenario { get; private set; }

        public ScenarioBuilder(Scenario scenario, Story story)
        {
            Scenario = scenario;
            _story = story;
        }

        private GivenFragment NewGivenFragment()
        {
            return new GivenFragment(Scenario, this);
        }

        public GivenFragment Given(string context, Action action)
        {
            _story.InvokeAction(GivenType, context, action);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0, Action<TArg0> action)
        {
            _story.InvokeAction(GivenType, context, action, arg0);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1> action)
        {
            _story.InvokeAction(GivenType, context, action, arg0, arg1);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                        Action<TArg0, TArg1, TArg2> action)
        {
            _story.InvokeAction(GivenType, context, action, arg0, arg1, arg2);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                               TArg3 arg3, Action<TArg0, TArg1, TArg2, TArg3> action)
        {
            _story.InvokeAction(GivenType, context, action, arg0, arg1, arg2, arg3);
            return NewGivenFragment();
        }

        public GivenFragment Given(string context)
        {
            _story.InvokeActionFromCatalog(GivenType, context);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0>(string context, TArg0 arg0)
        {
            _story.InvokeActionFromCatalog(GivenType, context, arg0);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1>(string context, TArg0 arg0, TArg1 arg1)
        {
            _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1, TArg2>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2);
            return NewGivenFragment();
        }

        public GivenFragment Given<TArg0, TArg1, TArg2, TArg3>(string context, TArg0 arg0, TArg1 arg1, TArg2 arg2,
                                                               TArg3 arg3)
        {
            _story.InvokeActionFromCatalog(GivenType, context, arg0, arg1, arg2, arg3);
            return NewGivenFragment();
        }
     
        public ScenarioBuilder Pending(string reason)
        {
            Scenario.Pending(reason);
            return this;
        }
    }
}
