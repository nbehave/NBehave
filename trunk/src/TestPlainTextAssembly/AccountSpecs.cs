using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using TestAssembly;

namespace TestPlainTextAssembly
{
    [Theme("Account transfers and deposits")]
    public class AccountSpecs
    {
        [Story]
        public void Transfer_to_cash_account()
        {
            Account savings = null;
            Account cash = null;

            var transferStory = new Story("Transfer to cash account");

            transferStory
                .AsA("savings account holder")
                .IWant("to transfer money from my savings account")
                .SoThat("I can get cash easily from an ATM");

            transferStory
                .WithScenario("Savings account is in credit")
                .Given("my savings account balance is $balance", 100, accountBalance => savings = new Account(accountBalance))
                .And("my cash account balance is $balance", 10, accountBalance => cash = new Account(accountBalance))
                .When("I transfer $amount to cash account", 20, transferAmount => savings.TransferTo(cash, transferAmount))
                .Then("my savings account balance should be $balance", 80, expectedBalance => savings.Balance.ShouldEqual(expectedBalance))
                .And("my cash account balance should be $balance", 30, expectedBalance => cash.Balance.ShouldEqual(expectedBalance));

            transferStory
                .WithScenario("Savings account is in credit with text")
                .Given("my savings account balance is 50")
                .And("my cash account balance is 20")
                .When("I transfer 20 to cash account")
                .Then("my savings account balance should be 30")
                .And("my cash account balance should be 40");
        }
    }
}