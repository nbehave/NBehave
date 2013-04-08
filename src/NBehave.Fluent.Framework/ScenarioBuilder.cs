using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NBehave.Hooks;
using Mono.Reflection;

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
        private readonly HooksCatalog hooksCatalog;
        int stepsCalled;
        private int stepsToRunBeforeAfterScenario;

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
            hooksCatalog = new HooksCatalog();
            SetupHooks();
        }

        private void SetupHooks()
        {
            var h = new HooksParser(hooksCatalog);
            h.FindHooks(GetTypeByCallStack());
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
            {
                RunHooks<BeforeScenarioAttribute>();
                CountStepsToRunBeforeCallingAfterScenario();
            }
            stepsCalled++;
            var stringStep = AddStepToScenario(currentStage, step);
            RunStep(currentStage, step, stringStep);

            RunAfterScenario();

            previousStage = currentStage;
            var failure = stringStep.StepResult.Result as Failed;
            if (failure != null)
            {
                throw new ApplicationException("Failed on step " + step, failure.Exception);
            }
        }

        private readonly Type[] virtualCallsTo = new[] { typeof(IGivenFragment), typeof(IWhenFragment), typeof(IGivenFragment) };
        private bool CallsNBehave(Instruction i)
        {
            var callvirt = i.OpCode.Name == "callvirt";
            if (callvirt)
            {
                var m = (MethodInfo)i.Operand;
                var returnType = m.ReturnParameter;
                if (returnType != null)
                    return virtualCallsTo.Contains(returnType.ParameterType);
            }
            return false;
        }

        private StringStep AddStepToScenario(ScenarioFragment currentStage, string step)
        {
            var stringStep = CreateStringStep(currentStage, step);
            Scenario.AddStep(stringStep);
            return stringStep;
        }

        private void RunStep(ScenarioFragment currentStage, string step, StringStep stringStep)
        {
            var stepToRun = new StringStep(currentStage.ToString(), step, Scenario.Source);
            try
            {
                RunHooks<BeforeStepAttribute>();
            }
            catch (Exception e)
            {
                stringStep.StepResult = new StepResult(stringStep, new Failed(e));
            }
            try
            {
                StepRunner.Run(stepToRun);
                stringStep.StepResult = new StepResult(stringStep, stepToRun.StepResult.Result);
            }
            finally
            {
                RunHooks<AfterStepAttribute>();
            }
        }

        private void RunAfterScenario()
        {
            if (stepsCalled == stepsToRunBeforeAfterScenario)
            {
                stepsCalled = 0;
                stepsToRunBeforeAfterScenario = -1;
                RunHooks<AfterScenarioAttribute>();
            }
        }

        private void RunHooks<T>()
        {
            hooksCatalog.OfType<T>().ToList().ForEach(_ => _.Invoke());
        }

        private StringStep CreateStringStep(ScenarioFragment currentStage, string step)
        {
            string stepType = currentStage.ToString();
            if (Scenario.Steps.Any() && previousStage == currentStage)
                stepType = "And";
            var stringStep = new StringStep(stepType, step, Scenario.Source);
            return stringStep;
        }

        private Assembly GetTypeByCallStack()
        {
            int i = -1;
            Type declaringType;
            do
            {
                i++;
                var stackFrame = new StackFrame(i);
                declaringType = stackFrame.GetMethod().DeclaringType;
            } while (declaringType.Assembly == typeof(ScenarioBuilder).Assembly);
            return declaringType.Assembly;
        }

        private void CountStepsToRunBeforeCallingAfterScenario()
        {
            int i = -1;
            StackFrame stackFrame;
            do
            {
                i++;
                stackFrame = new StackFrame(i);
            } while (stackFrame.GetMethod().DeclaringType.Assembly == typeof(ScenarioBuilder).Assembly);

            var instructions = Disassembler.GetInstructions(stackFrame.GetMethod())
                .Where(CallsNBehave)
                .ToList();
            stepsToRunBeforeAfterScenario = instructions.Count;
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
