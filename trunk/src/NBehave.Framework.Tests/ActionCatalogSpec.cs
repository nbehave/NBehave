using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class when_adding_an_action_to_the_catalog
    {
        [Specification]
        public void should_consider_the_2_actions_as_equal()
        {
            var catalog = new ActionCatalog();

            catalog.Add("my savings account balance is $balance", new object());
            bool actionExists = catalog.ActionExists("my savings account balance is 500");

            Assert.That(actionExists, Is.True);
        }

        [Specification]
        public void should_consider_all_whitespace_as_equal()
        {
            var catalog = new ActionCatalog();

            catalog.Add("my savings account\nbalance is $balance", new object());
            bool actionExists = catalog.ActionExists("my\tsavings account balance is 500");

            Assert.That(actionExists, Is.True);
        }

        [Specification]
        public void should_get_action()
        {
            var catalog = new ActionCatalog();

            catalog.Add("my savings account balance is $balance", new object());
            object action = catalog.GetAction("my savings account balance is 500");

            Assert.That(action, Is.Not.Null);            
        }

        [Specification]
        public void should_get_action_with_token_in_middle_of_string()
        {
            // Add an else to fix this: "I transfer 20 to cash account"
            var catalog = new ActionCatalog();
            Account cashAccount;
            Action<int> action = accountBalance => { cashAccount = new Account(accountBalance); };
            catalog.Add("I have $amount euros on my cash account", action);
            object actionFetched = catalog.GetAction("I have 20 euros on my cash account");

            Assert.That(actionFetched, Is.Not.Null);
        }

        [Specification]
        public void should_get_parameter_for_action_with_token_in_middle_of_string()
        {
            // Add an else to fix this: "I transfer 20 to cash account"
            var catalog = new ActionCatalog();
            Account cashAccount;
            Action<int> action = accountBalance => { cashAccount = new Account(accountBalance); };
            catalog.Add("I have $amount euros on my cash account", action);
            object[] values = catalog.GetParametersForMessage("I have 20 euros on my cash account");

            Assert.That(values.Length, Is.EqualTo(1));
            Assert.That(values[0].GetType(), Is.EqualTo(typeof(int)));
        }
    }
}
