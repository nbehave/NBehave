using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.Extensions
{
    [TestFixture]
    public class ActionStepConverterExtensionsSpec
    {
        [Test]
        public void Should_convert_string_with_variable_to_regex()
        {
            var regex = "a $variable".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_containing_numbers_to_regex()
        {
            var regex = "a $variable42".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable42>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_containing_only_one_character_regex()
        {
            var regex = "a $v".AsRegex();
            Assert.AreEqual(@"^a\s+(?<v>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_surronded_with_square_brackets_to_regex()
        {
            var regex = "a [variable]".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_one_char_variable_surronded_with_square_brackets_to_regex()
        {
            var regex = "a [v]".AsRegex();
            Assert.AreEqual(@"^a\s+(?<v>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_at_end_and_length_specifier_to_regex()
        {
            var regex = "a $variable{3,5}".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.{3,5})\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_and_length_specifier_to_regex()
        {
            var regex = "a $variable{3,5} x".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.{3,5})\s+x\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_in_Square_brackets_and_length_specifier_to_regex()
        {
            var regex = "a [variable]{3,5}".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.{3,5})\s*$", regex.ToString());
        }

        [Test]
        public void Should_convert_string_with_variable_surronded_with_angle_brackets_to_regex()
        {
            var regex = "a <variable>".AsRegex();
            Assert.AreEqual(@"^a\s+(?<variable>.+)\s*$", regex.ToString());
        }

        [Test]
        public void Should_escape_parentheses()
        {
            var regex = "handle () in regex".AsRegex();
            Assert.AreEqual(@"^handle\s+\(\)\s+in\s+regex\s*$", regex.ToString());            
        }

        [Test]
        public void Should_handle_parameter_between_parentheses()
        {
            var regex = "I toggle the cell at ($x, $y)".AsRegex();
            Assert.AreEqual(@"^I\s+toggle\s+the\s+cell\s+at\s+\((?<x>.+),\s+(?<y>.+)\)\s*$", regex.ToString());            
        }

        [TestCase("this is regex$")]
        [TestCase("^this is regex")]
        [TestCase(@"the grid should be\s+(?<rows>(.+\s*)+)")]
        public void Should_be_regex(string text)
        {
            Assert.IsTrue(text.IsRegex());
        }

        [TestCase("I have $parameter")]
        [TestCase((string)null)]
        public void Should_not_be_regex(string text)
        {
            Assert.IsFalse(text.IsRegex());
        }
    }
}