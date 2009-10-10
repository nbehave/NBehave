using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rhino.Mocks;
using Specification = NUnit.Framework.TestAttribute;


namespace NBehave.Narrator.Framework.Specifications
{
	[TestFixture]
	public class ScenarioStepRunnerSpec
	{
		private ScenarioStepRunner _runner;
		private ActionCatalog _actionCatalog;

		[SetUp]
		public void SetUp()
		{
			var actionStep = new ActionStep();
			var actionStepAlias = new ActionStepAlias();
			var textToTokenStringParser = new TextToTokenStringsParser(actionStepAlias, actionStep);
			_actionCatalog = new ActionCatalog();
			var actionSteprunner = new ActionStepRunner(_actionCatalog);
			_runner = new ScenarioStepRunner(textToTokenStringParser, actionSteprunner, actionStep);
		}

		
		public class When_running_a_scenario : ScenarioStepRunnerSpec
		{
			[Test]
			public void Should_have_result_for_each_step()
			{
				Action<string> action = name => Assert.AreEqual("Morgan", name);
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method));

				ScenarioResult scenarioResult = _runner.RunScenario(
					new ScenarioSteps
					{
						Steps =
							"Given my name is Axel" + Environment.NewLine +
							"And my name is Morgan"
					}, 1);

				Assert.AreEqual(2, scenarioResult.ActionStepResults.Count());
			}

			
			[Test]
			public void Should_have_different_result_for_each_step()
			{
				Action<string> action = name => Assert.AreEqual("Morgan", name);
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method));

				ScenarioResult scenarioResult = _runner.RunScenario(
					new ScenarioSteps
					{
						Steps =
							"Given my name is Morgan" + Environment.NewLine +
							"Given my name is Axel"
					}, 1);

				Assert.That(scenarioResult.ActionStepResults.First().Result,
				            Is.TypeOf(typeof(Passed)));
				Assert.That(scenarioResult.ActionStepResults.Last().Result,
				            Is.TypeOf(typeof(Failed)));
			}
		}
		
		
		public class When_Running_scenario_stream_with_multiple_scenarios : ScenarioStepRunnerSpec
		{
			[Test]
			public void Should_only_call_eventlistener_once_for_each_given()
			{
				Action<string> action = name => Assert.AreEqual("Morgan", name);
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method));

				ScenarioSteps fooScenario = new ScenarioSteps {
					Steps =
						"Scenario: foo" + Environment.NewLine +
						"Given foo" + Environment.NewLine +
						"When foo" + Environment.NewLine +
						"Then foo"};
				ScenarioSteps barScenario = new ScenarioSteps {
					Steps =
						"Scenario: bar" + Environment.NewLine +
						"Given bar" + Environment.NewLine +
						"When bar" + Environment.NewLine +
						"Then bar"};

				StoryResults storyResults = new StoryResults();

				IEventListener evtListener = MockRepository.GenerateMock<IEventListener>();
				_runner.EventListener = evtListener;
				_runner.RunScenarios(new List<ScenarioSteps> { fooScenario, barScenario}, storyResults);

				evtListener.AssertWasCalled(f=>f.ScenarioMessageAdded("Given foo - PENDING"));
				evtListener.AssertWasCalled(f=>f.ScenarioMessageAdded("Given bar - PENDING"));
				
				StringAssert.DoesNotContain("foo", storyResults.ScenarioResults[1].Message);
			}
		}

		
		[ActionSteps, TestFixture]
		public class When_running_many_scenarios_and_class_with_actionSteps_implements_notification_attributes : ScenarioStepRunnerSpec
		{
			private int _timesBeforeScenarioWasCalled;
			private int _timesBeforeStepWasCalled;
			private int _timesAfterStepWasCalled;
			private int _timesAfterScenarioWasCalled;

			[Given(@"something")]
			public void Given_something()
			{ }

			[BeforeScenario]
			public void OnBeforeScenario()
			{
				_timesBeforeScenarioWasCalled++;
			}

			[BeforeStep]
			public void OnBeforeStep()
			{
				_timesBeforeStepWasCalled++;
			}

			[AfterStep]
			public void OnAfterStep()
			{
				_timesAfterStepWasCalled++;
			}

			[AfterScenario]
			public void OnAfterScenario()
			{
				_timesAfterScenarioWasCalled++;
			}
			
			[TestFixtureSetUpAttribute]
			public void Setup()
			{
				base.SetUp();
				Action action = Given_something;
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"something to count$"), action, action.Method, this));
				
				ScenarioSteps firstScenario = new ScenarioSteps {
					Steps =
						"Scenario: One" + Environment.NewLine +
						"Given something to count"};
				ScenarioSteps secondScenario = new ScenarioSteps {
					Steps =
						"Scenario: Two" + Environment.NewLine +
						"Given something to count" + Environment.NewLine +
						"Given something to count"};

				_runner.EventListener = MockRepository.GenerateStub<IEventListener>();
				var storyResults = new StoryResults();
				_runner.RunScenarios(new List<ScenarioSteps> { firstScenario, secondScenario}, storyResults);
			}

			[Specification]
			public void should_Call_before_Scenario_once_per_scenario()
			{
				Assert.That(_timesBeforeScenarioWasCalled, Is.EqualTo(2));
			}

			[Specification]
			public void should_Call_after_Scenario_once_per_scenario()
			{
				Assert.That(_timesAfterScenarioWasCalled, Is.EqualTo(2));
			}
			
			[Specification]
			public void should_Call_before_step_once_per_step()
			{
				Assert.That(_timesBeforeStepWasCalled, Is.EqualTo(3));
			}

			[Specification]
			public void should_call_after_step_once_per_step()
			{
				Assert.That(_timesAfterStepWasCalled, Is.EqualTo(3));
			}
		}
	}
}