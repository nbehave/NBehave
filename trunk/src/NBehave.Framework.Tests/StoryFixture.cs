using System;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
	[TestFixture]
	public class StoryFixture
	{
		[Test]
		public void Story_raises_StoryCreated_event()
		{
			EventArgs<Story> eventArgs = null;
			Story.StoryCreated += ((sender, e) => eventArgs = e);
			new Story("Transfer to cash account");
			Assert.IsNotNull(eventArgs);
			Assert.That(eventArgs.EventData.Title, Is.EqualTo("Transfer to cash account"));
		}

		[Test]
		public void Story_sends_narrative_to_MessageAddedEvent()
		{
			string narrative = string.Empty;
			Story.MessageAdded += (sender, e) => narrative += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);
			new Story("Transfer to cash account")
				.AsA("Account Holder")
				.IWant("to withdraw cash from an ATM")
				.SoThat("I can get money when the bank is closed");
			Assert.AreEqual("Narrative: As a Account Holder" + Environment.NewLine +
			                "Narrative: I want to withdraw cash from an ATM" + Environment.NewLine +
			                "Narrative: So that I can get money when the bank is closed" + Environment.NewLine
			                , narrative);
		}

		[Test]
		public void ScenarioCreated_event_has_title_set()
		{
			string message = string.Empty;
			Story.ScenarioCreated += (sender, e) => message = e.EventData.Title;

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds");
			Assert.AreEqual("Account has sufficient funds", message);
		}

		[Test]
		public void Scenario_writes_title()
		{
			string actual = string.Empty;
			Story.ScenarioCreated += (sender, e) => actual += string.Format("Scenario: {0}{1}", e.EventData.Title, Environment.NewLine);
			//Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("", () => { })
				.When("", () => { })
				.Then("", () => { })
				.WithScenario("Account has insufficient funds");

			Assert.AreEqual("Scenario: Account has sufficient funds" + Environment.NewLine +
			                "Scenario: Account has insufficient funds" + Environment.NewLine
			                , actual);
		}

		[Test]
		public void Scenario_given_writes_value()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 20, accountBalance => { });

			Assert.AreEqual(
				"Given: Given the account balance is: 20" + Environment.NewLine, actual);
		}

		[Test]
		public void Scenario_given_accepts_multiple_inputs()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Something with addresses")
				.WithScenario("Address has some values")
				.Given("the address is", "123 anywhere", "Austin", "TX",
				       (address1, city, state) => { });

			Assert.AreEqual(
				"Given: Given the address is: (123 anywhere, Austin, TX)" + Environment.NewLine, actual);
		}

		[Test]
		public void Given_and_writes_value()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 20, accountBalance => { })
				.And("the card is valid", "4111111111111111", number => { });

			Assert.AreEqual(
				"Given: Given the account balance is: 20" + Environment.NewLine +
				"And: And the card is valid: 4111111111111111" + Environment.NewLine, actual);
		}

		[Test]
		public void Given_and_accepts_multiple_inputs()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Something with addresses")
				.WithScenario("Address has some values")
				.Given("the address is", "123 anywhere", "Austin", "TX", (address1, city, state) => { })
				.And("the other address is", "456 anywhere", "New York", "NY", (address1, city, state) => { });

			Assert.AreEqual(
				"Given: Given the address is: (123 anywhere, Austin, TX)" + Environment.NewLine +
				"And: And the other address is: (456 anywhere, New York, NY)" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Story_matches_titles_with_actions_inside_one_scenario()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			ScenarioBuilder scenario = new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds");

			scenario.Given("the account balance is", 20, accountBalance => { });
			scenario.Given("the account balance is", 30);

			Assert.AreEqual(
				"Given: Given the account balance is: 20" + Environment.NewLine +
				"Given: Given the account balance is: 30" + Environment.NewLine
				, actual);

		}

		[Test, ExpectedException(typeof(ActionMissingException))]
		public void Story_with_unmatched_title_throws_exception()
		{
			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 20);
			Assert.Fail("Should throw an exception");
		}

		[Test]
		public void Story_matches_titles_with_actions_between_many_scenarios()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			var story = new Story("Transfer to cash account");

			story.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 20, accountBalance => { });

			story.WithScenario("Account has insufficient funds")
				.Given("the account balance is", 30);

			Assert.AreEqual(
				"Given: Given the account balance is: 20" + Environment.NewLine +
				"Given: Given the account balance is: 30" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Scenario_when_writes_value()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 40, accountBalance => { })
				.When("the account holder requests", 20, requestAmount => { });

			Assert.AreEqual(
				"Given: Given the account balance is: 40" + Environment.NewLine +
				"When: When the account holder requests: 20" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Given_when_accepts_multiple_inputs()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Something with addresses")
				.WithScenario("Address has some values")
				.Given("the address is", "123 anywhere", "Austin", "TX", (address1, city, state) => { })
				.When("the other address is", "456 anywhere", "New York", "NY", (address1, city, state) => { });

			Assert.AreEqual(
				"Given: Given the address is: (123 anywhere, Austin, TX)" + Environment.NewLine +
				"When: When the other address is: (456 anywhere, New York, NY)" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Scenario_then_writes_value()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 40, accountBalance => { })
				.When("the account holder requests", 10, requestAmount => { })
				.Then("the account balance should be", 30, expectedBalance => Assert.IsTrue(true));

			Assert.AreEqual(
				"Given: Given the account balance is: 40" + Environment.NewLine +
				"When: When the account holder requests: 10" + Environment.NewLine +
				"Then: Then the account balance should be: 30" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Scenario_given_when_then_back_to_given_writes_value()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Given("the account balance is", 40, accountBalance => { })
				.When("the account holder requests", 10, requestAmount => { })
				.Then("the account balance should be", 30, expectedBalance => Assert.IsTrue(true))
				.Given("the account balance is", 20);

			Assert.AreEqual(
				"Given: Given the account balance is: 40" + Environment.NewLine +
				"When: When the account holder requests: 10" + Environment.NewLine +
				"Then: Then the account balance should be: 30" + Environment.NewLine +
				"Given: Given the account balance is: 20" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Failing_scenarios_write_failures()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			try
			{
				new Story("Transfer to cash account")
					.WithScenario("Account has sufficient funds")
					.Given("the account balance is", 40, accountBalance => { })
					.When("the account holder requests", 10, requestAmount => { })
					.Then("the account balance should be", 30, obj => { throw new Exception("Error"); });
			}
			catch { }

			Assert.AreEqual(
				"Given: Given the account balance is: 40" + Environment.NewLine +
				"When: When the account holder requests: 10" + Environment.NewLine +
				"Then: Then the account balance should be: 30 - FAILED" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Pending_scenarios_write_pending_reason()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Pending("needs an Account");

			Assert.AreEqual(
				"Pending: needs an Account" + Environment.NewLine
				, actual);
		}

		[Test]
		public void Pending_scenarios_dont_write_new_messages()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			new Story("Transfer to cash account")
				.WithScenario("Account has sufficient funds")
				.Pending("needs an Account")
				.Given("the account balance is", 40)
				.When("the account holder requests", 10)
				.Then("the account balance should be", 30);

			Assert.IsTrue(actual.StartsWith("Pending: needs an Account" + Environment.NewLine));
		}

		[Test]
		public void Should_set_the_story_title_when_created()
		{
			var story = new Story("Title");

			Assert.That(story.Title, Is.EqualTo("Title"));
		}

		[Test]
		public void Should_report_passing_scenario_into_story_results()
		{
			var story = new Story("Title");

			story.WithScenario("Test title");

			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(1));
			Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(0));
			Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(0));
		}

		[Test]
		public void Should_report_pending_scenario_into_story_results()
		{
			var story = new Story("Title");

			story.WithScenario("Test title")
				.Pending("Pending reason");

			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(0));
			Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(0));
			Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
		}

		[Test]
		public void Should_report_failing_scenario_into_story_results()
		{
			var story = new Story("Title");

			try
			{
				story.WithScenario("Test title")
					.Given("Throwing exception", delegate { throw new Exception(); });
			}
			catch { }

			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(0));
			Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
			Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(0));
		}

		[Test]
		public void Should_include_exception_message_with_failing_scenario()
		{
			var story = new Story("Title");

			try
			{
				story.WithScenario("Test title")
					.Given("Throwing exception", delegate { throw new Exception("Message"); });
			}
			catch { }

			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.ScenarioResults[0].Message, Is.EqualTo("System.Exception : Message"));
		}

		[Test]
		public void Should_include_pending_message_with_pending_scenario()
		{
			var story = new Story("Title");

			story.WithScenario("Test title")
				.Pending("reason");

			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.ScenarioResults[0].Message, Is.EqualTo("reason"));
		}

		[Test]
		public void Should_report_multiple_scenarios_into_story_results()
		{
			var story = new Story("Title");

			try
			{
				story.WithScenario("Failing Scenario")
					.Given("Throwing exception", delegate { throw new Exception(); })
					.When("Nothing", () => { })
					.Then("Nothing", () => { });
			}
			catch { }

			story.WithScenario("Passing Scenario")
				.Given("Nothing", () => { })
				.When("Nothing", () => { })
				.Then("Nothing", () => { });

			story.WithScenario("Pending Scenario")
				.Pending("Nothing");

			story.WithScenario("Passing Scenario")
				.Given("Nothing", () => { })
				.When("Nothing", () => { })
				.Then("Nothing", () => { });


			var results = new StoryResults();

			story.CompileResults(results);

			Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
			Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
			Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
		}

		[Test]
		public void Should_ignore_pending_and_errors_for_dry_runs()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			var story = new Story("Transfer to cash account");
			story.IsDryRun = true;

			story
				.WithScenario("Account has sufficient funds")
				.Pending("needs an Account")
				.Given("the account balance is", 40)
				.When("the account holder requests", 10)
				.Then("the account balance should be", 30)
				.Given("the account balance is", 20, delegate { throw new Exception("error"); });

			Assert.AreEqual(
				"Given: Given the account balance is: 40" + Environment.NewLine +
				"When: When the account holder requests: 10" + Environment.NewLine +
				"Then: Then the account balance should be: 30" + Environment.NewLine +
				"Given: Given the account balance is: 20" + Environment.NewLine
				, actual);

		}

		[Test]
		public void Should_invoke_cataloged_actions_which_has_no_parameters()
		{
			string actual = string.Empty;
			Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

			var story = new Story("Simple story");
			int runActionCounter = 0;

			story
				.WithScenario("Scenario that counts how many times actions run")
				.Given("initialized counter by", 0, x => runActionCounter = x)
				.When("increase counter by action", () => runActionCounter++)
				.And("increase counter by action")
				.Then("counter should be", 2, x => Assert.That(runActionCounter, Is.EqualTo(x)));

			Assert.AreEqual(
				"Given: Given initialized counter by: 0" + Environment.NewLine +
				"When: When increase counter by action" + Environment.NewLine +
				"And: And increase counter by action" + Environment.NewLine +
				"Then: Then counter should be: 2" + Environment.NewLine
				, actual);
		}

		[Context]
		public class When_using_tokenized_stories
		{
			[Specification]
			public void should_replace_token_with_value()
			{
				string actual = string.Empty;
				Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

				new Story("Transfer to cash account")
					.WithScenario("Account has sufficient funds")
					.Given("the account balance is $balance", 20, accountBalance => { });

				Assert.AreEqual(
					"Given: Given the account balance is 20" + Environment.NewLine
					, actual);
			}

			[Specification]
			public void should_not_add_value_to_reused_token()
			{
				string actual = string.Empty;
				Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

				new Story("Transfer to cash account")
					.WithScenario("Account has sufficient funds")
					.Given("the account balance is $balance", 20, accountBalance => { })
					.And("the account balance is 30");

				Assert.AreEqual(
					"Given: Given the account balance is 20" + Environment.NewLine +
					"And: And the account balance is 30" + Environment.NewLine
					, actual);

			}
		}

		[Context]
		public class When_using_given_with_no_parameters
		{
			[Specification]
			public void should_have_given_in_storys_MessageAdded_event()
			{
				string actual = string.Empty;
				Story.MessageAdded += (sender, e) => actual += string.Format("{0}: {1}{2}", e.EventData.Type, e.EventData.Message, Environment.NewLine);

				new Story("No parameters in given")
					.WithScenario("Given not registered")
					.Given("something");

				Assert.AreEqual(
					"Given: Given something - PENDING" + Environment.NewLine
					, actual);
				
			}
		}
	}

	public class Account
	{
		private int accountBalance;

		public Account(int accountBalance)
		{
			this.accountBalance = accountBalance;
		}

		public int Balance
		{
			get { return accountBalance; }
			set { accountBalance = value; }
		}

		public void TransferTo(Account account, int amount)
		{
			if (accountBalance > 0)
			{
				account.Balance = account.Balance + amount;
				Balance = Balance - amount;
			}
		}

		public void Withdraw(int amount)
		{
			if (amount > Balance)
				throw new ArgumentException("Amount exceeds balance", "amount");

			Balance -= amount;
		}
	}
}
