using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            ActionValue action = catalog.GetAction("my savings account balance is 500");

            Assert.That(action, Is.Not.Null);            
        }

        [Specification]
        public void should_get_action_with_token_in_middle_of_string()
        {
            var catalog = new ActionCatalog();
            Action<int> action = accountBalance =>  { };
            catalog.Add("I have $amount euros on my cash account", action);
            ActionValue actionFetched = catalog.GetAction("I have 20 euros on my cash account");

            Assert.That(actionFetched, Is.Not.Null);
        }

        [Specification]
        public void should_get_parameter_for_action_with_token_in_middle_of_string()
        {
            var catalog = new ActionCatalog();
            Action<int> action = accountBalance => { };
            catalog.Add("I have $amount euros on my cash account", action);
            object[] values = catalog.GetParametersForMessage("I have 20 euros on my cash account");

            Assert.That(values.Length, Is.EqualTo(1));
            Assert.That(values[0].GetType(), Is.EqualTo(typeof(int)));
        }

        [Specification]
        public void should_get_parameter_for_action_if_token_has_newlines()
        {
            var catalog = new ActionCatalog();
            Action<string> action = someAction => { };
            catalog.Add("I have a board like this\n$board", action);
            object[] values = catalog.GetParametersForMessage("I have a board like this\nxo \n x \no x");

            Assert.That(values.Length, Is.EqualTo(1));
            Assert.That(values[0], Is.EqualTo("xo \n x \no x"));
        }

        [Specification]
        public void should_get_parameters_for_message_with_action_registered_twice()
        {
            var catalog = new ActionCatalog();
            Action<string> action = someAction => { };
            catalog.Add("Given $value something", action);
            catalog.Add("And $value something", action);
            object[] givenValue = catalog.GetParametersForMessage("Given 20 something");
            object[] andValue = catalog.GetParametersForMessage("And 20 something");

            Assert.That(givenValue.Length, Is.EqualTo(1));
            Assert.That(andValue.Length, Is.EqualTo(1));
        }
    }
}
