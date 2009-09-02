using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionStepParserSpec
    {
        private ActionStepParser _runner;
        private ActionCatalog _actionCatalog;

        [SetUp]
        public void SetUp()
        {
            var storyRunnerFilter = new StoryRunnerFilter(".", ".", ".");
            _actionCatalog = new ActionCatalog();
            var actionStepAlias = new ActionStepAlias();
            _runner = new ActionStepParser(storyRunnerFilter, _actionCatalog);
            _runner.FindActionSteps(GetType().Assembly);
        }

        [Context, ActionSteps]
        public class When_having_ActionStepAttribute_multiple_times_on_same_method : ActionStepParserSpec
        {
            [ActionStep("Given one")]
            [ActionStep("Given two")]
            public void Multiple()
            {
                Assert.IsTrue(true);
            }

            [Specification]
            public void Should_find_action_using_first_actionStep_attribute_match()
            {
                ActionValue actionValue = _actionCatalog.GetAction("one");
                Assert.That(actionValue, Is.Not.Null);
            }

            [Specification]
            public void Should_find_action_using_second_actionStep_attribute_match()
            {
                ActionValue actionValue = _actionCatalog.GetAction("two");
                Assert.That(actionValue, Is.Not.Null);
            }
        }

        [Context, ActionSteps]
        public class When_having_ActionStepAttribute_without_tokenString : ActionStepParserSpec
        {
            [ActionStep]
            public void Given_a_method_with_no_parameters()
            {
                Assert.IsTrue(true);
            }

            [ActionStep]
            public void Given_a_method_with_a_value_intParam_plus_text_stringParam(int intParam, string stringParam)
            { }

            [Specification]
            public void Should_infer_parameters_in_tokenString_from_parameterNames_in_method()
            {
                var parameters = _actionCatalog.GetParametersForMessage("a method with a value 42 plus text stringParam");
                Assert.That(parameters[0], Is.EqualTo(42));
                Assert.That(parameters[1], Is.EqualTo("stringParam"));
            }
        }
    }
}