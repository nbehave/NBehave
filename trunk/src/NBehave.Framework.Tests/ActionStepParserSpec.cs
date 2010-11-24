using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionStepParserSpec
    {
        private ActionStepParser _actionStepParser;
        private ActionCatalog _actionCatalog;
        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter(".", ".", ".");
        private ParameterConverter _parameterConverter;

        [SetUp]
        public virtual void SetUp()
        {
            _actionCatalog = new ActionCatalog();
            _parameterConverter = new ParameterConverter(_actionCatalog);
            _actionStepParser = new ActionStepParser(_storyRunnerFilter, _actionCatalog);
            _actionStepParser.FindActionSteps(GetType().Assembly);
        }

        [Context, ActionSteps]
        public class When_having_ActionStepAttribute_multiple_times_on_same_method : ActionStepParserSpec
        {
            [Given("one")]
            [Given("two")]
            public void Multiple()
            {
                Assert.IsTrue(true);
            }

            [Specification]
            public void Should_find_action_using_first_actionStep_attribute_match()
            {
                ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("one", ""));
                Assert.That(action, Is.Not.Null);
            }

            [Specification]
            public void Should_find_action_using_second_actionStep_attribute_match()
            {
                ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("two", ""));
                Assert.That(action, Is.Not.Null);
            }
        }

        [Context, ActionSteps]
        public class When_having_ActionStepAttribute_without_tokenString : ActionStepParserSpec
        {
            [Given]
            public void Given_a_method_with_no_parameters()
            {
                Assert.IsTrue(true);
            }

            [Given]
            public void Given_a_method_with_a_value_intParam_plus_text_stringParam(int intParam, string stringParam)
            { }

            [Given]
            public void Given_using_GivenAttribute_with_no_regex()
            { }

            [When]
            public void When_using_WhenAttribute_with_no_regex()
            { }

            [Then]
            public void Then_using_ThenAttribute_with_no_regex()
            { }

            [Specification]
            public void Should_find_given_step_with_GivenAttribute()
            {
                var actionStepToFind = new ActionStepText("using GivenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Given"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+GivenAttribute\s+with\s+no\s+regex\s*$"));
            }

            [Specification]
            public void Should_find_when_step_with_WhenAttribute()
            {
                var actionStepToFind = new ActionStepText("using WhenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("When"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+WhenAttribute\s+with\s+no\s+regex\s*$"));
            }

            [Specification]
            public void Should_find_then_step_with_ThenAttribute()
            {
                var actionStepToFind = new ActionStepText("using ThenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Then"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+ThenAttribute\s+with\s+no\s+regex\s*$"));
            }
        }

        [Context, ActionSteps]
        public class When_having_ActionStepAttribute_with_tokenString : ActionStepParserSpec
        {
            [Given("a method with tokenstring and two parameters, one int value $intParam plus text $stringParam")]
            public void Given_a_method_with_a_value_intParam_plus_text_stringParam(int intParam, string stringParam)
            { }

            [Given("a method with \"embedded\" parameter like \"$param\" should work")]
            public void EmbeddedParam(string param)
            {

            }

            [Given("a length restriction on the \"$param{0,3}\" should work")]
            public void RestrictedLengthParam(string param)
            {

            }

            [Specification]
            public void Should_match_parameters_in_tokenString_to_method_parameters()
            {
                ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("a method with tokenstring and two parameters, one int value 42 plus text thistext", ""));

                Assert.That(action.ParameterInfo.GetLength(0), Is.EqualTo(2));
                Assert.That(action.ParameterInfo[0].ParameterType.Name, Is.EqualTo(typeof(int).Name));
                Assert.That(action.ParameterInfo[1].ParameterType.Name, Is.EqualTo(typeof(string).Name));
            }

            [Specification]
            public void Should_find_given_step_with_GivenAttribute()
            {
                var actionStepToFind = new ActionStepText("a method with tokenstring and two parameters, one int value 42 plus text thistext", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Given"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^a\s+method\s+with\s+tokenstring\s+and\s+two\s+parameters,\s+one\s+int\s+value\s+(?<intParam>.+)\s+plus\s+text\s+(?<stringParam>.+)\s*$"));
            }

            [Specification]
            public void Should_find_given_with_embedded_param()
            {
                var actionStepToFind = new ActionStepText("a method with \"embedded\" parameter like \"this\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Not.Null);
            }


            [Specification]
            public void Should_match_short_text_against_the_restricted_length_parameter()
            {
                var actionStepToFind = new ActionStepText("a length restriction on the \"txt\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Not.Null);
            }

            [Specification]
            public void Should_not_match_long_text_against_the_restricted_length_parameter()
            {
                var actionStepToFind = new ActionStepText("a length restriction on the \"supplied value\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Null);
            }
        }

        [Context, ActionSteps]
        public class When_class_with_actionSteps_attribute_implements_IMatchFiles : ActionStepParserSpec, IMatchFiles, IFileMatcher
        {
            [Given(@"something$")]
            public void Given_something()
            { }

            const string FileNameToMatch = "FileNameToMatch";
            private static string _wasCalledWithFileName = "";

            IFileMatcher IMatchFiles.FileMatcher
            {
                get { return this; }
            }

            bool IFileMatcher.IsMatch(string fileName)
            {
                _wasCalledWithFileName = fileName;
                return FileNameToMatch.Equals(fileName);
            }

            [Specification]
            public void Should_match_filename()
            {
                ActionStepText actionStepText = new ActionStepText("something", FileNameToMatch);
                Assert.IsTrue(_actionCatalog.ActionExists(actionStepText));
            }

            [Specification]
            public void Should_call_IsMatch_on_interface_with_correct_fileName()
            {
                ActionStepText actionStepText = new ActionStepText("Given something", FileNameToMatch);
                _actionCatalog.ActionExists(actionStepText);
                Assert.That(_wasCalledWithFileName, Is.EqualTo(FileNameToMatch));
            }
        }


        [Context]
        public class When_having_ActionStepAttribute_on_abstract_class : ActionStepParserSpec
        {
            [ActionSteps]
            public abstract class AbstaractBase
            {
                [Given("one abstract$")]
                public void Multiple()
                {
                    Assert.IsTrue(true);
                }
            }

            public class Inheritor : AbstaractBase
            {

            }

            public override void SetUp()
            {
                _storyRunnerFilter = new StoryRunnerFilter(GetType().Namespace, ".", ".");
                base.SetUp();
            }

            [Specification]
            public void Should_find_action_using_first_actionStep_attribute_match()
            {
                ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("one abstract", ""));
                Assert.That(action, Is.Not.Null);
            }
        }
    }
}