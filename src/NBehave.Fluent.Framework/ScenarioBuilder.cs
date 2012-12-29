using System;
using System.Linq;
using NBehave.Narrator.Framework;

namespace NBehave.Fluent.Framework
{
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
        private Scenario _scenario;
        private readonly string _scenarioTitle;
        private ScenarioFragment previousStage = ScenarioFragment.Given;

        protected Feature Feature { get; private set; }

        protected Scenario Scenario
        {
            get
            {
                if (_scenario == null)
                {
                    _scenario = new Scenario(_scenarioTitle, "");
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
            if (inlineImplementation != null)
                StepRunner.RegisterImplementation(currentStage, step, inlineImplementation);

            if (!Scenario.Steps.Any())
                StepRunner.BeforeScenario();

            var stringStep = AddStepToScenario(currentStage, step);
            RunStep(currentStage, step, stringStep);

            previousStage = currentStage;
            var failure = stringStep.StepResult.Result as Failed;
            if (failure != null)
            {
                throw new ApplicationException("Failed on step " + step, failure.Exception);
            }
        }

        private StringStep AddStepToScenario(ScenarioFragment currentStage, string step)
        {
            var stringStep = CreateStringStep(step, currentStage);
            Scenario.AddStep(stringStep);
            return stringStep;
        }

        private void RunStep(ScenarioFragment currentStage, string step, StringStep stringStep)
        {
            var stepToRun = new StringStep(string.Format("{0} {1}", currentStage, step), Scenario.Source);
            StepRunner.Run(stepToRun);
            stringStep.StepResult = new StepResult(stringStep, stepToRun.StepResult.Result);
        }

        private StringStep CreateStringStep(string step, ScenarioFragment currentStage)
        {
            string stepType = currentStage.ToString();
            if (Scenario.Steps.Any() && previousStage == currentStage)
                stepType = "And";
            var stringStep = new StringStep(string.Format("{0} {1}", stepType, step), Scenario.Source);
            return stringStep;
        }

        internal class StartFragment : IScenarioBuilderStartWithHelperObject
        {
            private readonly ScenarioBuilder _builder;

            public StartFragment(Feature feature)
                : this(feature, null)
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
