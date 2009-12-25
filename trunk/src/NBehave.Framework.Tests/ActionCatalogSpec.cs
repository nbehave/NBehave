using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionCatalogSpec
    {
        private MethodInfo GetDummyParameterInfo()
        {
            Action<int> a = p => { };
            return a.Method;
        }

        [Context]
        public class Valid_parameter_names : ActionCatalogSpec
        {
            private readonly ActionCatalog _actionCatalog = new ActionCatalog();

            [Specification]
            public void Should_consider_any_character_in_english_alphabet_as_valid()
            {
                string message = _actionCatalog.BuildMessage("valid $parameterName", new[] { "parameter" });
                Assert.AreEqual("valid parameter", message);
            }

            [Test]
            public void Should_consider_any_character_in_english_alphabet_mixed_with_numbers_as_valid()
            {
                string message = _actionCatalog.BuildMessage("valid $parameter1Name2", new[] { "parameter" });
                Assert.AreEqual("valid parameter", message);
            }

            [Test]
            public void Should_consider_any_character_in_english_alphabet_mixed_with_underscore_valid()
            {
                string message = _actionCatalog.BuildMessage("valid $parameter_Name", new[] { "parameter" });
                Assert.AreEqual("valid parameter", message);
            }

            [Test]
            public void Should_not_consider_parameter_name_as_valid_if_it_starts_with_a_number()
            {
                string message = _actionCatalog.BuildMessage("valid $1parameter1Name2", new[] { "parameter" });
                Assert.AreEqual("valid $1parameter1Name2", message);
            }

            [Test]
            public void Should_not_consider_space_as_part_of_parameter_name()
            {
                string message = _actionCatalog.BuildMessage("valid $parameterName it is", new[] { "parameter" });
                Assert.AreEqual("valid parameter it is", message);
            }

            [Test]
            public void Should_consider_parameter_name_enclosed_in_square_brackets_as_valid()
            {
                string message = _actionCatalog.BuildMessage("valid [parameter1Name2]", new[] { "parameter" });
                Assert.AreEqual("valid parameter", message);
            }

            [Test]
            public void Should_be_able_to_embedd_a_parameter_inside_non_alphabetic_characters()
            {
                string message = _actionCatalog.BuildMessage("I should see a message, \"$message\"", new[] { "Hello, Morgan" });
                Assert.AreEqual("I should see a message, \"Hello, Morgan\"", message);
            }
        }

        [TestFixture]
        public class when_adding_an_action_to_the_catalog : ActionCatalogSpec
        {
            [Test]
            public void should_consider_the_2_actions_as_equal()
            {
                var catalog = new ActionCatalog();
                catalog.Add("my savings account balance is $balance", new object(), GetDummyParameterInfo());
                bool actionExists = catalog.ActionExists("my savings account balance is 500");

                Assert.That(actionExists, Is.True);
            }

            [Test]
            public void should_consider_all_whitespace_as_equal()
            {
                var catalog = new ActionCatalog();

                catalog.Add("my savings account\nbalance is $balance", new object(), GetDummyParameterInfo());
                bool actionExists = catalog.ActionExists("my\tsavings account balance is 500");

                Assert.That(actionExists, Is.True);
            }

            [Test]
            public void should_get_action()
            {
                var catalog = new ActionCatalog();

                catalog.Add("my savings account balance is $balance", new object(), GetDummyParameterInfo());
                ActionMethodInfo action = catalog.GetAction(new ActionStepText("my savings account balance is 500", ""));

                Assert.That(action, Is.Not.Null);
            }

            [Test]
            public void should_get_action_with_token_in_middle_of_string()
            {
                var catalog = new ActionCatalog();
                Action<int> action = accountBalance => { };
                catalog.Add("I have $amount euros on my cash account", action, GetDummyParameterInfo());
                ActionMethodInfo actionFetched = catalog.GetAction(new ActionStepText("I have 20 euros on my cash account", ""));

                Assert.That(actionFetched, Is.Not.Null);
            }
        }

        [TestFixture]
        public class When_fetching_parameters_for_actionStep : ActionCatalogSpec
        {
            private ActionCatalog _actionCatalog;

            [SetUp]
            public void Establish_context()
            {
                _actionCatalog = new ActionCatalog();
            }

            [Test]
            public void should_get_parameter_for_action_with_token_in_middle_of_string()
            {
                var catalog = new ActionCatalog();
                Action<int> action = accountBalance => { };
                catalog.Add("I have $amount euros on my cash account", action, action.Method);
                object[] values = catalog.GetParametersForActionStepText(new ActionStepText("I have 20 euros on my cash account", ""));

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0].GetType(), Is.EqualTo(typeof(int)));
            }

            [Test]
            public void should_get_parameter_for_action_if_token_has_newlines()
            {
                var catalog = new ActionCatalog();
                Action<string> action = someAction => { };
                catalog.Add("I have a board like this\n$board", action, action.Method);
                object[] values = catalog.GetParametersForActionStepText(new ActionStepText("I have a board like this\nxo \n x \no x", ""));

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0], Is.EqualTo("xo \n x \no x"));
            }

            [Test]
            public void should_get_parameters_for_message_with_action_registered_twice()
            {
                var catalog = new ActionCatalog();
                Action<string> action = someAction => { };
                catalog.Add("Given $value something", action, action.Method);
                catalog.Add("And $value something", action, action.Method);
                object[] givenValue = catalog.GetParametersForActionStepText(new ActionStepText("Given 20 something", ""));
                object[] andValue = catalog.GetParametersForActionStepText(new ActionStepText("And 20 something", ""));

                Assert.That(givenValue.Length, Is.EqualTo(1));
                Assert.That(andValue.Length, Is.EqualTo(1));
            }

            [Test]
            public void should_get_parameters_for_message_with_a_negative_parameter()
            {
                var catalog = new ActionCatalog();
                Action<string> action = someAction => { };
                catalog.Add("Given $value something", action, action.Method);
                object[] givenValue = catalog.GetParametersForActionStepText(new ActionStepText("Given -20 something", ""));

                Assert.That(givenValue.Length, Is.EqualTo(1));
                Assert.That(givenValue.First(), Is.EqualTo("-20"));
            }

            [Test]
            public void Should_get_int_parameter()
            {
                Action<int> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"an int (?<value>\d+)"), action, action.Method, "Given"));
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText("an int 42", ""));
                Assert.That(values[0], Is.TypeOf(typeof(int)));
            }

            [Test]
            public void Should_get_decimal_parameter()
            {
                Action<decimal> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a decimal (?<value>\d+)"), action, action.Method, "Given"));
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText("a decimal 42", ""));
                Assert.That(values[0], Is.TypeOf(typeof(decimal)));
            }

            [Test]
            public void Should_get_multiline_value_as_string()
            {
                Action<object> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a string\s+(?<value>(\w+\s+)*)"), action, action.Method, "Given"));
                string multiLineValue = "one" + Environment.NewLine + "two";
                string actionString = "a string " + multiLineValue;
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That(values[0], Is.TypeOf(typeof(string)));
            }

            [Test]
            public void Should_get_multiline_value_as_array_of_strings()
            {
                object paramReceived = null;
                Action<string[]> actionStep = p => { };
                Action<object> action = value => { paramReceived = value; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a string\s+(?<value>(\w+,?\s*)+)"), action, actionStep.Method, "Given"));
                string multiLineValue = "one, two";
                string actionString = "a string " + Environment.NewLine + multiLineValue;
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That(values[0], Is.TypeOf(typeof(string[])));
                var arr = (string[])values[0];
                Assert.AreEqual("one", arr[0]);
                Assert.AreEqual("two", arr[1]);
            }

            [Test]
            public void Should_remove_empty_entries_at_end_of_array_values()
            {
                Action<string[]> action = value => { };

                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a string\s+(?<value>(\w+,?\s*)+)"), action, action.Method, "Given"));
                string multiLineValue = "one,two," + Environment.NewLine;
                string actionString = "a string " + Environment.NewLine + multiLineValue;
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That((values[0] as string[]), Is.EqualTo(new[] { "one", "two" }));
            }

            [Test]
            public void Should_get_multiline_value_as_array_of_integers()
            {
                object paramReceived = null;
                Action<int[]> actionStep = p => { };
                Action<object> action = value => { paramReceived = value; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a list of integers (?<value>(\d+,?\s*)+)"), action, actionStep.Method, "Given"));
                string multiLineValue = "1, 2, 5";
                string actionString = "a list of integers " + multiLineValue;
                object[] values = _actionCatalog.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That(values[0], Is.TypeOf(typeof(int[])));
                var arr = (int[])values[0];
                Assert.AreEqual(1, arr[0]);
                Assert.AreEqual(2, arr[1]);
                Assert.AreEqual(5, arr[2]);
            }
        }

        [TestFixture]
        public class When_two_actions_match_the_same_text_step : ActionCatalogSpec
        {
            private ActionCatalog _actionCatalog;
            private bool _wasCalled;

            [SetUp]
            public void Establish_context()
            {
                _wasCalled = false;
                _actionCatalog = new ActionCatalog();
                Action firstAction = () => Assert.Fail("This action shouldnt be called");
                var actionMethod = new ActionMethodInfo(new Regex("def$"), firstAction, firstAction.Method, "Given", this);
                _actionCatalog.Add(actionMethod);

                Action secondAction = () => { _wasCalled = true; };
                var secondActionMethod = new ActionMethodInfo(new Regex("abc def$"), secondAction, secondAction.Method, "Given", this);
                _actionCatalog.Add(secondActionMethod);
            }

            private void Because_of()
            {
                var actionText = new ActionStepText("abc def", "somestory.story");
                ActionMethodInfo action = _actionCatalog.GetAction(actionText);
                (action.Action as Action).Invoke();
            }

            [Test]
            public void Should_call_greediest_matching_action()
            {
                Because_of();
                Assert.That(_wasCalled, Is.True);
            }
        }

        [TestFixture]
        public class When_fetching_parameters_with_row_value : ActionCatalogSpec
        {
            private ActionCatalog _actionCatalog;

            [SetUp]
            public void Establish_context()
            {
                _actionCatalog = new ActionCatalog();
            }

            [Test]
            public void should_get_parameter_for_action_with_token_in_middle_of_string()
            {
                var catalog = new ActionCatalog();
                Action<string> action = name => { };
                catalog.Add("I have a name", action, action.Method);
                Row row = new Row(new ExampleColumns(new[] { "name" }), new Dictionary<string, string> { { "name", "Morgan" } });
                object[] values = catalog.GetParametersForActionStepText(new ActionStepText("I have a name", ""), row);

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0].GetType(), Is.EqualTo(typeof(string)));
            }
        }
    }
}