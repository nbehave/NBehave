using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;


namespace NBehave.Narrator.Framework.Specifications
{
	[Context]
	public class ActionStepRunnerSpec
	{
		private ActionStepRunner _runner;
		private ActionCatalog _actionCatalog;

		[SetUp]
		public void SetUp()
		{
			_actionCatalog = new ActionCatalog();
			_runner = new ActionStepRunner(_actionCatalog);
		}

		[Context]
		public class When_running_plain_text_scenarios : ActionStepRunnerSpec
		{
			[Specification]
			public void Should_invoke_action_given_a_token_string()
			{
				bool wasCalled = false;
				Action<string> action = name => { wasCalled = true; };
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method));
				_runner.InvokeTokenString(new ActionStepText("my name is Morgan",""));
				Assert.IsTrue(wasCalled, "Action was not called");
			}

			[Specification]
			public void Should_get_parameter_value_for_action()
			{
				string actual = string.Empty;
				Action<string> action = name => { actual = name; };
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"my name is (?<name>\w+)"), action, action.Method));
				_runner.InvokeTokenString(new ActionStepText("my name is Morgan",""));
				Assert.That(actual, Is.EqualTo("Morgan"));
			}

			[Specification, ExpectedException(typeof(ArgumentException))]
			public void Should_throw_ArgumentException_if_action_given_in_token_string_doesnt_exist()
			{
				_runner.InvokeTokenString(new ActionStepText("This doesnt exist",""));
			}
		}

		[Context, ActionSteps]
		public class When_class_with_actionSteps_implements_notification_attributes : ActionStepRunnerSpec
		{
			private bool _beforeScenarioWasCalled;
			private bool _beforeStepWasCalled;
			private bool _afterStepWasCalled;
			private bool _afterScenarioWasCalled;

			[Given(@"something")]
			public void Given_something()
			{ }

			[BeforeScenario]
			public void OnBeforeScenario()
			{
				_beforeScenarioWasCalled = true;
			}

			[BeforeStep]
			public void OnBeforeStep()
			{
				_beforeStepWasCalled = true;
			}

			[AfterStep]
			public void OnAfterStep()
			{
				_afterStepWasCalled = true;
			}

			[AfterScenario]
			public void OnAfterScenario()
			{
				_afterScenarioWasCalled = true;
			}
			
			[SetUp]
			public void Setup()
			{
				SetUp();

				Action action = Given_something;
				_actionCatalog.Add(new ActionMethodInfo(new Regex(@"something"), action, action.Method, this));

				_beforeScenarioWasCalled = false;
				_beforeStepWasCalled = false;
				_afterStepWasCalled = false;
				_afterScenarioWasCalled = false;
			}

			[Specification]
			public void Running_a_step_should_call_most_attributed_methods()
			{
				var actionStepText = new ActionStepText("something", "");
				_runner.RunActionStepRow(actionStepText);
				
				Assert.That(_beforeScenarioWasCalled);
				Assert.That(_beforeStepWasCalled);
				Assert.That(_afterStepWasCalled);
				Assert.That(!_afterScenarioWasCalled);
			}

			[Specification]
			public void Completing_a_scenario_should_call_all_attributed_methods()
			{
				var actionStepText = new ActionStepText("something", "");
				_runner.RunActionStepRow(actionStepText);
				_runner.OnCloseScenario();

				Assert.That(_beforeScenarioWasCalled);
				Assert.That(_beforeStepWasCalled);
				Assert.That(_afterStepWasCalled);
				Assert.That(_afterScenarioWasCalled);
			}

		}
	}
}
