using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Processors
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
    }
}