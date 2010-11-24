using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ActionStepAttributeSpec
    {
        [Test]
        public void Should_ignore_space_at_End_of_step()
        {
            var g = new GivenAttribute("something");
            Assert.That(g.ActionMatch.IsMatch("something  "), Is.True);
        }
    }
}
