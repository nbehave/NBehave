using System;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class StoryFixture
    {
        [Test]
        public void Story_writes_title_to_messageprovider()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider);
            }
        }

        [Test]
        public void Story_writes_narrative_to_messageprovider()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("Narrative:");
                LastCall.Repeat.Once();
                provider.AddMessage("\tAs a Account Holder");
                LastCall.Repeat.Once();
                provider.AddMessage("\tI want to withdraw cash from an ATM");
                LastCall.Repeat.Once();
                provider.AddMessage("\tSo that I can get money when the bank is closed");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .AsA("Account Holder")
                    .IWant("to withdraw cash from an ATM")
                    .SoThat("I can get money when the bank is closed");
            }
        }

        [Test]
        public void Scenario_writes_title_to_messageprovider()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds");
            }
        }

        [Test]
        public void Scenario_writes_title_with_correct_numbers()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven ");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen ");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tThen ");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 2: Account has insufficient funds");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("", delegate { DoNothing(); })
                    .When("", delegate { DoNothing(); })
                    .Then("", delegate { DoNothing(); })
                    .WithScenario("Account has insufficient funds");
            }
        }

        [Test]
        public void Scenario_given_writes_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 20");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is", 20, delegate(int accountBalance) { account = new Account(accountBalance); });
            }
        }


        [Test]
        public void Scenario_given_accepts_multiple_inputs()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (MessageProviderRegistry.RegisterScopedInstance(provider))
            {

                using (repo.Record())
                {
                    provider.AddMessage("Story: Something with addresses");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 1: Address has some values");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the address is: (123 anywhere, Austin, TX)");
                    LastCall.Repeat.Once();
                }

                Address address = null;

                using (repo.Playback())
                {
                    new Story("Something with addresses", provider)
                        .WithScenario("Address has some values")
                        .Given("the address is", "123 anywhere", "Austin", "TX", delegate(string address1, string city, string state) { address = new Address(address1, city, state); });
                }

            }
        }

        [Test]
        public void Given_and_writes_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 20");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\t\tAnd the card is valid: 4111111111111111");
                LastCall.Repeat.Once();
            }

            Card card = null;
            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is", 20, delegate(int accountBalance) { account = new Account(accountBalance); })
                    .And("the card is valid", "4111111111111111", delegate(string number) { card = new Card(number); });
            }
        }

        [Test]
        public void Given_and_accepts_multiple_inputs()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (MessageProviderRegistry.RegisterScopedInstance(provider))
            {

                using (repo.Record())
                {
                    provider.AddMessage("Story: Something with addresses");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 1: Address has some values");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the address is: (123 anywhere, Austin, TX)");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\t\tAnd the other address is: (456 anywhere, New York, NY)");
                    LastCall.Repeat.Once();
                }

                Address address = null;

                using (repo.Playback())
                {
                    new Story("Something with addresses")
                        .WithScenario("Address has some values")
                        .Given("the address is", "123 anywhere", "Austin", "TX", delegate(string address1, string city, string state) { address = new Address(address1, city, state); })
                        .And("the other address is", "456 anywhere", "New York", "NY", delegate(string address1, string city, string state) { address = new Address(address1, city, state); });
                }

            }
        }

        [Test]
        public void Story_matches_titles_with_actions_inside_one_scenario()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (MessageProviderRegistry.RegisterScopedInstance(provider))
            {

                using (repo.Record())
                {
                    provider.AddMessage("Story: Transfer to cash account");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 1: Account has sufficient funds");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the account balance is: 20");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the account balance is: 30");
                    LastCall.Repeat.Once();
                }

                Account account = null;

                using (repo.Playback())
                {
                    Scenario scenario = new Story("Transfer to cash account", provider)
                        .WithScenario("Account has sufficient funds");

                    scenario.Given("the account balance is", 20, delegate(int accountBalance) { account = new Account(accountBalance); });

                    scenario.Given("the account balance is", 30);
                }

            }
        }

        [Test, ExpectedException(typeof(ActionMissingException))]
        public void Story_with_unmatched_title_throws_exception()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.Stub<IMessageProvider>();

            new Story("Transfer to cash account", provider)
                .WithScenario("Account has sufficient funds")
                .Given("the account balance is", 20);
        }

        [Test]
        public void Story_matches_titles_with_actions_between_many_scenarios()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (MessageProviderRegistry.RegisterScopedInstance(provider))
            {

                using (repo.Record())
                {
                    provider.AddMessage("Story: Transfer to cash account");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 1: Account has sufficient funds");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the account balance is: 20");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 2: Account has insufficient funds");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the account balance is: 30");
                    LastCall.Repeat.Once();
                }

                Account account = null;

                using (repo.Playback())
                {
                    Story story = new Story("Transfer to cash account", provider);

                    story.WithScenario("Account has sufficient funds")
                        .Given("the account balance is", 20, delegate(int accountBalance) { account = new Account(accountBalance); });

                    story.WithScenario("Account has insufficient funds")
                        .Given("the account balance is", 30);
                }

            }
        }

        [Test]
        public void Scenario_when_writes_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 40");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen the account holder requests: 20");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is", 40, delegate(int accountBalance) { account = new Account(accountBalance); })
                    .When("the account holder requests", 20, delegate(int requestAmount) { account.Withdraw(requestAmount); });
            }
        }

        [Test]
        public void Given_when_accepts_multiple_inputs()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (MessageProviderRegistry.RegisterScopedInstance(provider))
            {

                using (repo.Record())
                {
                    provider.AddMessage("Story: Something with addresses");
                    LastCall.Repeat.Once();
                    provider.AddMessage("");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\tScenario 1: Address has some values");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tGiven the address is: (123 anywhere, Austin, TX)");
                    LastCall.Repeat.Once();
                    provider.AddMessage("\t\tWhen the other address is: (456 anywhere, New York, NY)");
                    LastCall.Repeat.Once();
                }

                Address address = null;

                using (repo.Playback())
                {
                    new Story("Something with addresses")
                        .WithScenario("Address has some values")
                        .Given("the address is", "123 anywhere", "Austin", "TX", delegate(string address1, string city, string state) { address = new Address(address1, city, state); })
                        .When("the other address is", "456 anywhere", "New York", "NY", delegate(string address1, string city, string state) { address = new Address(address1, city, state); });
                }

            }
        }

        [Test]
        public void Scenario_then_writes_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 40");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen the account holder requests: 10");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tThen the account balance should be: 30");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is", 40, delegate(int accountBalance) { account = new Account(accountBalance); })
                    .When("the account holder requests", 10, delegate(int requestAmount) { account.Withdraw(requestAmount); })
                    .Then("the account balance should be", 30, delegate(int expectedBalance) { Assert.AreEqual(expectedBalance, account.Balance); });
            }
        }

        [Test]
        public void Scenario_given_when_then_back_to_given_writes_value()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 40");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen the account holder requests: 10");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tThen the account balance should be: 30");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 20");
                LastCall.Repeat.Once();
            }

            Account account = null;

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Given("the account balance is", 40, delegate(int accountBalance) { account = new Account(accountBalance); })
                    .When("the account holder requests", 10, delegate(int requestAmount) { account.Withdraw(requestAmount); })
                    .Then("the account balance should be", 30, delegate(int expectedBalance) { Assert.That(account.Balance, Is.EqualTo(expectedBalance)); })
                    .Given("the account balance is", 20);
            }
        }

        [Test]
        public void Failing_scenarios_write_failures()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 40");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen the account holder requests: 10");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tThen the account balance should be: 30 - FAILED");
            }

            Account account = null;

            using (repo.Playback())
            {
                try
                {
                    new Story("Transfer to cash account", provider)
                        .WithScenario("Account has sufficient funds")
                        .Given("the account balance is", 40,
                               delegate(int accountBalance) { account = new Account(accountBalance); })
                        .When("the account holder requests", 10,
                              delegate(int requestAmount) { account.Withdraw(requestAmount); })
                        .Then("the account balance should be", 30,
                              delegate { throw new Exception("Error"); });
                }
                catch { }
            }
        }

        [Test]
        public void Pending_scenarios_write_pending_reason()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tPending: needs an Account");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Pending("needs an Account");
            }
        }

        [Test]
        public void Pending_scenarios_dont_write_new_messages()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tPending: needs an Account");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                new Story("Transfer to cash account", provider)
                    .WithScenario("Account has sufficient funds")
                    .Pending("needs an Account")
                    .Given("the account balance is", 40)
                    .When("the account holder requests", 10)
                    .Then("the account balance should be", 30);
            }
        }

        [Test]
        public void Should_set_the_story_title_when_created()
        {
            Story story = new Story("Title");

            Assert.That(story.Title, Is.EqualTo("Title"));
        }

        [Test]
        public void Should_report_passing_scenario_into_story_results()
        {
            Story story = new Story("Title");

            story.WithScenario("Test title");

            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(1));
            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(0));
            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(0));
        }

        [Test]
        public void Should_report_pending_scenario_into_story_results()
        {
            Story story = new Story("Title");

            story.WithScenario("Test title")
                .Pending("Pending reason");

            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(0));
            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(0));
            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
        }

        [Test]
        public void Should_report_failing_scenario_into_story_results()
        {
            Story story = new Story("Title");

            try
            {
                story.WithScenario("Test title")
                    .Given("Throwing exception", delegate { throw new Exception(); });
            }
            catch { }

            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(0));
            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(0));
        }

        [Test]
        public void Should_include_exception_message_with_failing_scenario()
        {
            Story story = new Story("Title");

            try
            {
                story.WithScenario("Test title")
                    .Given("Throwing exception", delegate { throw new Exception("Message"); });
            }
            catch { }

            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.ScenarioResults[0].Message, Is.EqualTo("System.Exception : Message"));
        }

        [Test]
        public void Should_include_pending_message_with_pending_scenario()
        {
            Story story = new Story("Title");

            story.WithScenario("Test title")
                .Pending("reason");

            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.ScenarioResults[0].Message, Is.EqualTo("reason"));
        }

        [Test]
        public void Should_report_multiple_scenarios_into_story_results()
        {
            Story story = new Story("Title");

            try
            {
                story.WithScenario("Failing Scenario")
                    .Given("Throwing exception", delegate { throw new Exception(); })
                    .When("Nothing", DoNothing)
                    .Then("Nothing", DoNothing);
            }
            catch { }

            story.WithScenario("Passing Scenario")
                .Given("Nothing", DoNothing)
                .When("Nothing", DoNothing)
                .Then("Nothing", DoNothing);

            story.WithScenario("Pending Scenario")
                .Pending("Nothing");

            story.WithScenario("Passing Scenario")
                .Given("Nothing", DoNothing)
                .When("Nothing", DoNothing)
                .Then("Nothing", DoNothing);


            StoryResults results = new StoryResults();

            story.CompileResults(results);

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
        }

        [Test]
        public void Should_ignore_pending_and_errors_for_dry_runs()
        {
            MockRepository repo = new MockRepository();

            IMessageProvider provider = repo.CreateMock<IMessageProvider>();

            using (repo.Record())
            {
                provider.AddMessage("Story: Transfer to cash account");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\tScenario 1: Account has sufficient funds");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tPending: needs an Account");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 40");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tWhen the account holder requests: 10");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tThen the account balance should be: 30");
                LastCall.Repeat.Once();
                provider.AddMessage("");
                LastCall.Repeat.Once();
                provider.AddMessage("\t\tGiven the account balance is: 20");
                LastCall.Repeat.Once();
            }

            using (repo.Playback())
            {
                Story story = new Story("Transfer to cash account", provider);

                story.IsDryRun = true;

                story
                    .WithScenario("Account has sufficient funds")
                    .Pending("needs an Account")
                    .Given("the account balance is", 40)
                    .When("the account holder requests", 10)
                    .Then("the account balance should be", 30)
                    .Given("the account balance is", 20, delegate { throw new Exception("error"); });

            }
        }

        private void DoNothing()
        {
            // Placeholder for delegates
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

    public class Address
    {
        private readonly string _address1;
        private readonly string _city;
        private readonly string _state;

        public Address(string address1, string city, string state)
        {
            _address1 = address1;
            _city = city;
            _state = state;
        }
    }

    public class Card
    {
        private string number;

        public Card(string number)
        {
            this.number = number;
        }

        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        public bool IsValid()
        {
            return true;
        }
    }
}