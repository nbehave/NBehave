using System;
using System.Text.RegularExpressions;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;


namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionStepRunnerSpec
    {
        [Context]
        public class When_running_plain_text_scenarios : ActionStepRunnerSpec
        {
            private ActionStepRunner _runner;
            private ActionCatalog _actionCatalog;

            [SetUp]
            public void SetUp()
            {
                _actionCatalog = new ActionCatalog();
                _runner = new ActionStepRunner(_actionCatalog);
            }

            [Specification]
            public void Should_invoke_action_given_a_token_string()
            {
                bool wasCalled = false;
                Action<string> action = name => { wasCalled = true; };
                _actionCatalog.Add(new Regex(@"my name is (?<name>\w+)"), action);
                _runner.InvokeTokenString("my name is Morgan");
                Assert.IsTrue(wasCalled, "Action was not called");
            }

            [Specification]
            public void Should_get_parameter_value_for_action()
            {
                string actual = string.Empty;
                Action<string> action = name => { actual = name; };
                _actionCatalog.Add(new Regex(@"my name is (?<name>\w+)"), action);
                _runner.InvokeTokenString("my name is Morgan");
                Assert.That(actual, Is.EqualTo("Morgan"));
            }

            [Specification, ExpectedException(typeof(ArgumentException))]
            public void Should_throw_ArgumentException_if_action_given_in_token_string_doesnt_exist()
            {
                _runner.InvokeTokenString("This doesnt exist");
            }
        }
    }
}
