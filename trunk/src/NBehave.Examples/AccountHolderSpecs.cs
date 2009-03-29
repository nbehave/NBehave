using System.Collections.Generic;
using System.Text;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples
{
    [Theme("Account holder")]
    public class AccountHolderSpecs
    {
        [Story]
        public void Add_account_to_holder()
        {
            var story = new Story("Add account to holder");

            story
                .AsA("account holder")
                .IWant("to add accounts to my portfolio")
                .SoThat("I can access those accounts through the bank");

            Account account = null;
            AccountHolder holder = null;

            story.WithScenario("new account")
                .Given("my account holder is", "Joe", name => holder = new AccountHolder(name))
                .And("a new account with balance", 0, balance => account = new Account(balance))
                .When("the new account is added to the account holder", account,
                      accountToAdd => holder.AddAccount(accountToAdd))
                .Then("the new account should be held by the account holder", holder, account,
                      (accountHolder, accountHeld) => accountHolder.AccountsHeld[0].ShouldEqual(accountHeld))
                ;
        }
    }

    public class AccountHolder
    {
        public string Name
        {
            get { return _name; }
        }

        public Account[] AccountsHeld
        {
            get { return _accounts.ToArray(); }
        }

        private readonly string _name;
        private readonly List<Account> _accounts;

        public AccountHolder(string name)
        {
            _name = name;
            _accounts = new List<Account>();
        }

        public void AddAccount(Account account)
        {
            _accounts.Add(account);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Holder:'{0}' Accounts:[", Name);

            int num = 1;
            foreach (var account in _accounts)
            {
                if (num < _accounts.Count)
                    sb.AppendFormat("{0}:{1}, ", num, account.Balance);
                else
                    sb.AppendFormat("{0}:{1}", num, account.Balance);
                num++;
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}