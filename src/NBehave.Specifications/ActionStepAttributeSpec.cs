using System.Reflection;
using NUnit.Framework;

namespace NBehave.Specifications
{
    [TestFixture]
    public class ActionStepAttributeSpec
    {
        [Test]
        public void ShouldIgnoreSpaceAtEndOfStep()
        {
            var g = new GivenAttribute("something");
            Assert.That(g.ActionMatch.IsMatch("something  "), Is.True);
        }

        [Given]
        public void Given_multiple_choices_to_match_the_parameter_parameter_PARAMETER_parameter(string parameter)
        { }

        [Test]
        public void Should_pick_uppercase_word_as_parameter_if_multiple_words_match_parameter_name()
        {
            var a = new GivenAttribute();
            MethodInfo method = GetType().GetMethod("Given_multiple_choices_to_match_the_parameter_parameter_PARAMETER_parameter");
            a.BuildActionMatchFromMethodInfo(method);
            Assert.AreEqual(@"^multiple\s+choices\s+to\s+match\s+the\s+parameter\s+parameter\s+(?<parameter>.+)\s+parameter\s*$",
                a.ActionMatch.ToString());
        }
    }
}
