using System;
using System.Linq;
using NBehave.Narrator.Framework;

namespace NBehave.Spec
{
    using NBehave.Narrator.Framework.Tiny;

    public enum ScenarioFragment
    {
        Given,
        When,
        Then
    }

    public class ScenarioBuilder
    {
        private object _stepHelper;
        private ScenarioDrivenSpecStepRunner _stepRunner;
        private ScenarioWithSteps _scenario;
        private readonly string _scenarioTitle;

        protected Feature Feature { get; private set; }

        protected ScenarioWithSteps Scenario
        {
            get
            {
                if(_scenario == null)
                {
                    _scenario = new ScenarioWithSteps(StepRunner, TinyIoCContainer.Current.Resolve<ITinyMessengerHub>()) { Title = _scenarioTitle };
                    Feature.AddScenario(Scenario);
                }
                return _scenario;
            }
        }

        private ScenarioDrivenSpecStepRunner StepRunner
        {
            get { return _stepRunner ?? (_stepRunner = new ScenarioDrivenSpecStepRunner(_stepHelper)); }
        }

        public ScenarioBuilder(Feature feature, string scenarioTitle)
        {
            Feature = feature;
            _scenarioTitle = scenarioTitle;
        }

        private void SetHelperObject(object helper)
        {
            _stepHelper = helper;
        }

        private void AddStepAndExecute(ScenarioFragment currentStage, string step, Action inlineImplementation)
        {
            StepRunner.CurrentScenarioStage = currentStage;

            if (inlineImplementation != null)
                StepRunner.RegisterImplementation(step, inlineImplementation);

            if(Scenario.Steps.Count() == 0)
                StepRunner.BeforeScenario();

            var stringStringStep = new StringStep(step, Scenario.Source, StepRunner);
            Scenario.AddStep(stringStringStep);
            
            stringStringStep.Run();
            var failure = stringStringStep.StepResult.Result as Failed;
            if (failure != null)
            {
                throw new ApplicationException("Failed on step " + step, failure.Exception);
            }
        }

        internal class StartFragment : IScenarioBuilderStartWithHelperObject
        {
            private readonly ScenarioBuilder _builder;

            public StartFragment(Feature feature) : this(feature, null)
            {
            }

            public StartFragment(Feature feature, string scenarioTitle)
            {
                _builder = new ScenarioBuilder(feature, scenarioTitle);
            }

            public IGivenFragment Given(string step)
            {
                return Given(step, null);
            }

            public IGivenFragment Given(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.Given, step, implementation);
                return new GivenFragment(_builder);
            }

            public IScenarioBuilderStart WithHelperObject(object stepHelper)
            {
                _builder.SetHelperObject(stepHelper);
                return this;
            }

            public IScenarioBuilderStart WithHelperObject<T>() where T : new()
            {
                _builder.SetHelperObject(new T());
                return this;
            }
        }

        internal class GivenFragment : IGivenFragment
        {
            private readonly ScenarioBuilder _builder;

            public GivenFragment(ScenarioBuilder builder)
            {
                _builder = builder;
            }

            public IGivenFragment And(string step)
            {
                return And(step, null);
            }

            public IGivenFragment And(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.Given, step, implementation);
                return this;
            }

            public IWhenFragment When(string step)
            {
                return When(step, null);
            }

            public IWhenFragment When(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.When, step, implementation);
                return new WhenFragment(_builder);
            }
        }

        internal class WhenFragment : IWhenFragment
        {
            private readonly ScenarioBuilder _builder;

            public WhenFragment(ScenarioBuilder builder)
            {
                _builder = builder;
            }

            public IWhenFragment And(string step)
            {
                return And(step, null);
            }

            public IWhenFragment And(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.When, step, implementation);
                return this;
            }

            public IThenFragment Then(string step)
            {
                return Then(step, null);
            }

            public IThenFragment Then(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.Then, step, implementation);
                return new ThenFragment(_builder);
            }
        }

        internal class ThenFragment : IThenFragment
        {
            private readonly ScenarioBuilder _builder;

            public ThenFragment(ScenarioBuilder builder)
            {
                _builder = builder;
            }

            public IThenFragment And(string step)
            {
                return And(step, null);
            }

            public IThenFragment And(string step, Action implementation)
            {
                _builder.AddStepAndExecute(ScenarioFragment.Then, step, implementation);
                return this;
            }
        }
    }
}
