using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class ActionStepParserSpec
    {
        private ActionStepParser _actionStepParser;
        private ActionCatalog _actionCatalog;
        private StoryRunnerFilter _storyRunnerFilter = new StoryRunnerFilter(".", ".", ".");

        [SetUp]
        public virtual void SetUp()
        {
            _actionCatalog = new ActionCatalog();
            _actionStepParser = new ActionStepParser(_storyRunnerFilter, _actionCatalog);
            _actionStepParser.FindActionSteps(GetType().Assembly);
        }

        [TestFixture, ActionSteps]
        public class WhenHavingActionStepAttributeMultipleTimesOnSameMethod : ActionStepParserSpec
        {
            [Given("one")]
            [Given("two")]
            public void Multiple()
            {
                Assert.IsTrue(true);
            }

            [Test]
            public void ShouldFindActionUsingFirstActionStepAttributeMatch()
            {
                var action = _actionCatalog.GetAction(new StringStep("one", ""));
                Assert.That(action, Is.Not.Null);
            }

            [Test]
            public void ShouldFindActionUsingSecondActionStepAttributeMatch()
            {
                var action = _actionCatalog.GetAction(new StringStep("two", ""));
                Assert.That(action, Is.Not.Null);
            }
        }

        [TestFixture, ActionSteps]
        public class WhenHavingActionStepAttributeWithoutTokenString : ActionStepParserSpec
        {
            // ReSharper disable InconsistentNaming
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
            // ReSharper restore InconsistentNaming

            [Test]
            public void ShouldFindGivenStepWithGivenAttribute()
            {
                var actionStepToFind = new StringStep("Given using GivenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Given"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+GivenAttribute\s+with\s+no\s+regex\s*$"));
            }

            [Test]
            public void ShouldFindWhenStepWithWhenAttribute()
            {
                var actionStepToFind = new StringStep("Given using WhenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("When"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+WhenAttribute\s+with\s+no\s+regex\s*$"));
            }

            [Test]
            public void ShouldFindThenStepWithThenAttribute()
            {
                var actionStepToFind = new StringStep("Given using ThenAttribute with no regex", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Then"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^using\s+ThenAttribute\s+with\s+no\s+regex\s*$"));
            }
        }

        [TestFixture, ActionSteps]
        public class WhenHavingActionStepAttributeWithTokenString : ActionStepParserSpec
        {
            [Given("a method with tokenstring and two parameters, one int value $intParam plus text $stringParam")]
            public void GivenAMethodWithAValueIntParamPlusTextStringParam(int intParam, string stringParam)
            { }

            [Given("a method with \"embedded\" parameter like \"$param\" should work")]
            public void EmbeddedParam(string param)
            {

            }

            [Given("a length restriction on the \"$param{0,3}\" should work")]
            public void RestrictedLengthParam(string param)
            {

            }

            [Test]
            public void ShouldMatchParametersInTokenStringToMethodParameters()
            {
                var action = _actionCatalog.GetAction(new StringStep("Given a method with tokenstring and two parameters, one int value 42 plus text thistext", ""));

                Assert.That(action.ParameterInfo.GetLength(0), Is.EqualTo(2));
                Assert.That(action.ParameterInfo[0].ParameterType.Name, Is.EqualTo(typeof(int).Name));
                Assert.That(action.ParameterInfo[1].ParameterType.Name, Is.EqualTo(typeof(string).Name));
            }

            [Test]
            public void ShouldFindGivenStepWithGivenAttribute()
            {
                var actionStepToFind = new StringStep("Given a method with tokenstring and two parameters, one int value 42 plus text thistext", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action.ActionType, Is.EqualTo("Given"));
                Assert.That(action.ActionStepMatcher.ToString(), Is.EqualTo(@"^a\s+method\s+with\s+tokenstring\s+and\s+two\s+parameters,\s+one\s+int\s+value\s+(?<intParam>.+)\s+plus\s+text\s+(?<stringParam>.+)\s*$"));
            }

            [Test]
            public void ShouldFindGivenWithEmbeddedParam()
            {
                var actionStepToFind = new StringStep("Given a method with \"embedded\" parameter like \"this\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Not.Null);
            }

            [Test]
            public void ShouldMatchShortTextAgainstTheRestrictedLengthParameter()
            {
                var actionStepToFind = new StringStep("Given a length restriction on the \"txt\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Not.Null);
            }

            [Test]
            public void ShouldNotMatchLongTextAgainstTheRestrictedLengthParameter()
            {
                var actionStepToFind = new StringStep("Given a length restriction on the \"supplied value\" should work", "file");
                var action = _actionCatalog.GetAction(actionStepToFind);
                Assert.That(action, Is.Null);
            }
        }

        [TestFixture, ActionSteps]
        public class WhenClassWithActionStepsAttributeImplementsIMatchFiles : ActionStepParserSpec, IMatchFiles, IFileMatcher
        {
            [Given(@"something$")]
            public void GivenSomething()
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

            [Test]
            public void ShouldMatchFilename()
            {
                var actionStepText = new StringStep("Given something", FileNameToMatch);
                Assert.IsTrue(_actionCatalog.ActionExists(actionStepText));
            }

            [Test]
            public void ShouldCallIsMatchOnInterfaceWithCorrectFileName()
            {
                var actionStepText = new StringStep("Given something", FileNameToMatch);
                _actionCatalog.ActionExists(actionStepText);
                Assert.That(_wasCalledWithFileName, Is.EqualTo(FileNameToMatch));
            }
        }


        [TestFixture]
        public class WhenHavingActionStepAttributeOnAbstractClass : ActionStepParserSpec
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

            [Test]
            public void ShouldFindActionUsingFirstActionStepAttributeMatch()
            {
                var action = _actionCatalog.GetAction(new StringStep("Given one abstract", ""));
                Assert.That(action, Is.Not.Null);
            }
        }
    }
}