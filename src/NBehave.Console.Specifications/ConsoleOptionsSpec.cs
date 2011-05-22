using NUnit.Framework;

namespace NBehave.Console.Specifications
{
    [TestFixture]
    public class ConsoleOptionsSpec
    {
        [Test]
        public void parameter_scenarioFiles_is_required()
        {
            string[] args = { "foo.dll" };
            var options = new ConsoleOptions(args);
            var result = options.Validate();
            Assert.That(result, Is.False);
        }
    }
}