using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NBehave.Narrator.Framework;
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
    }
}