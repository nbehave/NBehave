using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Examples
{
    [Theme("Account holder")]
    public class AccountHolderSpecs
    {
        [Story]
        public void Add_account_to_holder()
        {
            
            Story story = new Story("Add account ot holder");

            story
                .AsA("account holder")
                .IWant("to add accounts to my portfolio")
                .SoThat("I can access those accounts through the bank");

            Account account = null;
            AccountHolder holder = null;

            story.WithScenario("new account")
                .Given("my account holder is", "Joe", delegate(string name) { holder = new AccountHolder(name); })
                    .And("a new account with balance", 0, delegate (int balance) { account = new Account(balance); } )
                .When("the new account is added to the account holder", account, 
                        delegate(Account accountToAdd) { holder.AddAccount(accountToAdd);} )
                .Then("the new account should be held by the account holder", holder, account, 
                    delegate (AccountHolder accountHolder, Account accountHeld) {
                        Assert.That(accountHolder.AccountsHeld[0], Is.EqualTo(accountHeld)); } )
            ;
        }
    }

    public class AccountHolder
    {
        public string Name
        {
            get { return name; }
        }
        public Account[] AccountsHeld
        {
            get
            {
                return accounts.ToArray();
            }
        }

        private readonly string name;
        private readonly List<Account> accounts;

        public AccountHolder(string name)
        {
            this.name = name;
            this.accounts = new List<Account>();
        }

        public void AddAccount(Account account)
        {
            this.accounts.Add(account);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Holder:'{0}' Accounts:[", Name);

            int num = 1;
            foreach (Account account in accounts)
            {
                if (num < accounts.Count)
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
