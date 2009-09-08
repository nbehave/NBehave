using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rhino.Mocks;
using NBehave.Narrator.Framework.EventListeners;

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
	}
}