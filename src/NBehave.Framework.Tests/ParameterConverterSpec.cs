using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ParameterConverterSpec
    {
        private ParameterConverter _parameterConverter;
        private ActionCatalog _actionCatalog;

        [SetUp]
        public void Establish_context()
        {
            _actionCatalog = new ActionCatalog();
            _parameterConverter = new ParameterConverter(_actionCatalog);
        }

        [TestFixture]
        public class When_fetching_parameters_for_actionStep : ParameterConverterSpec
        {
            [Test]
            public void should_get_parameter_for_action_with_token_in_middle_of_string()
            {
                Action<int> action = accountBalance => { };

                _actionCatalog.Add(new ActionMethodInfo("I have $amount euros on my cash account".AsRegex(), action, action.Method, null));
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText("I have 20 euros on my cash account", ""));

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0].GetType(), Is.EqualTo(typeof(int)));
            }

            [Test]
            public void should_get_parameter_for_action_if_token_has_newlines()
            {
                Action<string> action = someAction => { };
                _actionCatalog.Add(new ActionMethodInfo("I have a board like this\n$board".AsRegex(), action, action.Method, null));
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText("I have a board like this\nxo \n x \no x", ""));

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0], Is.EqualTo("xo \n x \no x"));
            }

            [Test]
            public void should_get_parameters_for_message_with_action_registered_twice()
            {
                Action<string> action = someAction => { };
                _actionCatalog.Add(new ActionMethodInfo("Given $value something".AsRegex(), action, action.Method, null));
                _actionCatalog.Add(new ActionMethodInfo("And $value something".AsRegex(), action, action.Method, null));
                var givenValue = _parameterConverter.GetParametersForActionStepText(new ActionStepText("Given 20 something", ""));
                var andValue = _parameterConverter.GetParametersForActionStepText(new ActionStepText("And 20 something", ""));

                Assert.That(givenValue.Length, Is.EqualTo(1));
                Assert.That(andValue.Length, Is.EqualTo(1));
            }

            [Test]
            public void should_get_parameters_for_message_with_a_negative_parameter()
            {
                Action<string> action = someAction => { };
                _actionCatalog.Add(new ActionMethodInfo("Given $value something".AsRegex(), action, action.Method, null));
                var givenValue = _parameterConverter.GetParametersForActionStepText(new ActionStepText("Given -20 something", ""));

                Assert.That(givenValue.Length, Is.EqualTo(1));
                Assert.That(givenValue.First(), Is.EqualTo("-20"));
            }

            [Test]
            public void Should_get_int_parameter()
            {
                Action<int> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"an int (?<value>\d+)"), action, action.Method, "Given"));
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText("an int 42", ""));
                Assert.That(values[0], Is.TypeOf(typeof(int)));
            }

            [Test]
            public void Should_get_decimal_parameter()
            {
                Action<decimal> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a decimal (?<value>\d+)"), action, action.Method, "Given"));
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText("a decimal 42", ""));
                Assert.That(values[0], Is.TypeOf(typeof(decimal)));
            }

            [Test]
            public void Should_get_multiline_value_as_string()
            {
                Action<object> action = value => { };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a string\s+(?<value>(\w+\s+)*)"), action, action.Method, "Given"));
                var multiLineValue = "one" + Environment.NewLine + "two";
                var actionString = "a string " + multiLineValue;
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That(values[0], Is.TypeOf(typeof(string)));
            }

            [Test]
            public void Should_get_multiline_value_as_array_of_strings()
            {
                object paramReceived = null;
                Action<string[]> actionStep = p => { };
                Action<object> action = value => { paramReceived = value; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a string\s+(?<value>(\w+,?\s*)+)"), action, actionStep.Method, "Given"));
                const string multiLineValue = "one, two";
                var actionString = "a string " + Environment.NewLine + multiLineValue;
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText(actionString, ""));
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
                var multiLineValue = "one,two," + Environment.NewLine;
                var actionString = "a string " + Environment.NewLine + multiLineValue;
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That((values[0] as string[]), Is.EqualTo(new[] { "one", "two" }));
            }

            [Test]
            public void Should_get_multiline_value_as_array_of_integers()
            {
                Should_get_multiline_value_as_generic_collection_of_integers<int[]>();
            }

            [Test]
            public void Should_get_multiline_value_as_generic_IEnumerable_of_integers()
            {
                Should_get_multiline_value_as_generic_collection_of_integers<IEnumerable<int>>();
            }

            [Test]
            public void Should_get_multiline_value_as_generic_ICollection_of_integers()
            {
                Should_get_multiline_value_as_generic_collection_of_integers<ICollection<int>>();
            }

            [Test]
            public void Should_get_multiline_value_as_generic_IList_of_integers()
            {
                Should_get_multiline_value_as_generic_collection_of_integers<IList<int>>();
            }

            [Test]
            public void Should_get_multiline_value_as_generic_List_of_integers()
            {
                Should_get_multiline_value_as_generic_collection_of_integers<List<int>>();
            }

            public void Should_get_multiline_value_as_generic_collection_of_integers<T>() where T : IEnumerable<int>
            {
                object paramReceived = null;
                Action<T> actionStep = p => { };
                Action<object> action = value => { paramReceived = value; };
                _actionCatalog.Add(new ActionMethodInfo(new Regex(@"a list of integers (?<value>(\d+,?\s*)+)"), action, actionStep.Method, "Given"));
                var multiLineValue = "1, 2, 5";
                var actionString = "a list of integers " + multiLineValue;
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText(actionString, ""));
                Assert.That(values[0], Is.AssignableTo(typeof(T)));
                var arr = (T)values[0];
                Assert.AreEqual(1, arr.First());
                Assert.AreEqual(2, arr.Skip(1).First());
                Assert.AreEqual(5, arr.Last());
            }
        }

        [TestFixture]
        public class When_fetching_parameters_with_row_value : ParameterConverterSpec
        {
            [Test]
            public void should_get_parameter_for_action_with_token_in_middle_of_string()
            {
                Action<string> action = name => { };
                _actionCatalog.Add(new ActionMethodInfo("I have a name".AsRegex(), action, action.Method, null));
                var row = new Row(new ExampleColumns(new[] { "name" }), new Dictionary<string, string> { { "name", "Morgan" } });
                var values = _parameterConverter.GetParametersForActionStepText(new ActionStepText("I have a name", ""), row);

                Assert.That(values.Length, Is.EqualTo(1));
                Assert.That(values[0].GetType(), Is.EqualTo(typeof(string)));
            }
        }
    }
}