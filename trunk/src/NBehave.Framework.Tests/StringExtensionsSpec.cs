using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class StringExtensionsSpec
    {
        [Test]
        public void Should_remove_first_word_when_first_char_is_not_whitespace()
        {
            var str = "one two";
            Assert.That(str.RemoveFirstWord(), Is.EqualTo("two"));
        }

        [Test]
        public void Should_remove_first_word_when_first_char_is_a_whitespace()
        {
            var str = "\tone two";
            Assert.That(str.RemoveFirstWord(), Is.EqualTo("two"));
        }
    }
}