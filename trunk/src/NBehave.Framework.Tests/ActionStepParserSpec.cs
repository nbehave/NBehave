using NUnit.Framework;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
	[Context]
	public class ActionStepParserSpec
	{
		private ActionStepParser _actionStepParser;
		private ActionCatalog _actionCatalog;

		[SetUp]
		public void SetUp()
		{
			var storyRunnerFilter = new StoryRunnerFilter(".", ".", ".");
			_actionCatalog = new ActionCatalog();
			var actionStepAlias = new ActionStepAlias();
			_actionStepParser = new ActionStepParser(storyRunnerFilter, _actionCatalog);
			_actionStepParser.FindActionSteps(GetType().Assembly);
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
				ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("one",""));
				Assert.That(action, Is.Not.Null);
			}

			[Specification]
			public void Should_find_action_using_second_actionStep_attribute_match()
			{
				ActionMethodInfo action = _actionCatalog.GetAction(new ActionStepText("two",""));
				Assert.That(action, Is.Not.Null);
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
				var parameters = _actionCatalog.GetParametersForActionStepText(new ActionStepText("a method with a value 42 plus text stringParam",""));
				Assert.That(parameters[0], Is.EqualTo(42));
				Assert.That(parameters[1], Is.EqualTo("stringParam"));
			}
		}
		
		[Context, ActionSteps]
		public class When_class_with_actionSteps_attribute_implements_IMatchFiles : ActionStepParserSpec, IMatchFiles, IFileMatcher
		{
			[Given(@"something")]
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
				ActionStepText actionStepText = new ActionStepText("something",FileNameToMatch);
				Assert.IsTrue(_actionCatalog.ActionExists(actionStepText));
			}

			[Specification]
			public void Should_call_IsMatch_on_interface_with_correct_fileName()
			{
				ActionStepText actionStepText = new ActionStepText("Given something",FileNameToMatch);
				_actionCatalog.ActionExists(actionStepText);
				Assert.That(_wasCalledWithFileName, Is.EqualTo(FileNameToMatch));
			}
		}
	}
}