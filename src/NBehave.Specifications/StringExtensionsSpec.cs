using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications
{
    [TestFixture]
    public class StringExtensionsSpec
    {
        [Test]
        public void ShouldRemoveFirstWordWhenFirstCharIsNotWhitespace()
        {
            const string str = "one two";
            Assert.That(str.RemoveFirstWord(), Is.EqualTo("two"));
        }

        [Test]
        public void ShouldRemoveFirstWordWhenFirstCharIsAWhitespace()
        {
            const string str = "\tone two";
            Assert.That(str.RemoveFirstWord(), Is.EqualTo("two"));
        }
    }
}